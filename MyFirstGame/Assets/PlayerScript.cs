using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class PlayerScript : MonoBehaviour
{
    #region notes
    //Instaid of hardcoding ifstatements, make a method to perform an action or return multiple values.
    //volicity is gelijk aan snelheid en de richting waar een object in beweegd
    //void: does not return a value, can be defined independent of the class
    //private void: does not return a value, can only be used in the same class
    //public void: does not return a value, can be used outside the class
    #endregion

    #region fields
    //serialize zodat je de waarde in unity kan veranderen
    //kan niet door scripts worden aangespast
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;

    //startlocatie van player
    private Vector2 startLocation = Vector3.zero;

    //jumping height player
    private float jumpingPower = 10f;
    private float slideSpeed = 20f;
    //duration of slide in seconds
    private float slideDuration = 0.8f;
    //timer to manage slide duration
    private float slideTimer = 0f;
    //to track if player is falling
    private float fallPosition = -20f;


    //Vector 2 voor x en y input (vector3 is xyz)
    //makes player move on x and y axis 
    //Method OnMove haald/update de waardes van Vector2 op 
    //De waardes van OnMove worden in field moveInput opgeslagen
    Vector2 moveInput;

    //set body Player
    Rigidbody2D body;

    //used in method oncollision
    bool enemyHit = false;
    //to track if savepoint is reached
    bool hasReachedSavePoint = false;
    //to track if player is sliding
    bool isSliding = false;
    //to track if player is falling
    bool isFalling = false;

    //audio
    private AudioSource audioSource;
    private AudioClip jumpClip;
    private AudioClip backGroundClip;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        startLocation = body.position;

        //audio source background in start update for a constant sound when playing
        //add audio component to gameobject
        audioSource = gameObject.AddComponent<AudioSource>();

        //load audio from resource folder
        jumpClip = Resources.Load<AudioClip>("jump");
        backGroundClip = Resources.Load<AudioClip>("background");

        //configure audiosource to play music on loop
        audioSource.clip = backGroundClip;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //reset hele scene
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene("LevelOne");
        }

        #region movementPlayer 

        //bepaald welke kant player beweegd op basis van moveinput 
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, body.velocity.y);


        //als de speler op W klikt en de grond word geraakt, dan springt player
        if (moveInput.y > 0 && IsGrounded())
        {
            playerVelocity = new Vector2(moveInput.x, jumpingPower);
            animator.SetBool("Jump", true);
            audioSource.PlayOneShot(jumpClip); //play sound when jumping
        }
        else
        {
            animator.SetBool("Jump", false);
        }

        //als speler op S klikt, de grond word geraakt en nog niet slide:
        if (Input.GetKey(KeyCode.S) && IsGrounded() && !isSliding)
        {
            isSliding = true;
            slideTimer = slideDuration; //set slidetimer to slideduration to enable counting slideduration
            //slide logic 
            playerVelocity = new Vector2(Mathf.Sign(transform.localScale.x) * slideSpeed, playerVelocity.y);
            //set paramater in unity to true
            animator.SetBool("preslide", true);
            animator.SetBool("slide", true);
        }
        if (isSliding)
        {
            slideTimer -= Time.deltaTime; //delta time for decrementing slidetimer
            if (slideTimer < 0) //when slide duration reaches 0, stop sliding
            {
                isSliding = false;
                animator.SetBool("preslide", false);
                animator.SetBool("slide", false); //set parameter unity to false
            }
        }

        body.velocity = playerVelocity;
        //controleerd of player beweegd
        bool playerMoves = Mathf.Abs(playerVelocity.x) > Mathf.Epsilon;
        if (playerMoves)
        {
            animator.SetBool("Run", true);
            //draait speler op basis hoe speler beweegd
            transform.localScale = new Vector2(Mathf.Sign(body.velocity.x), transform.localScale.y);
        }
        else
        {
            animator.SetBool("Run", false);
        }
        #endregion

        //wanneer speler vijand raakt, update nieuwe begin positie wanneer savepoint is bereikt. 
        if (enemyHit)
        {
            if (hasReachedSavePoint)
            {
                RespawnAtSavePoint();
            }
            else
            {
                RespawnAtStart();
            }
        }

        CheckIfFalling();
        HandleFalling();       

    }

    void OnMove(InputValue value)
    {
        //Hier worden de waardes van Vector2 opgehaald/geupdate. 
        moveInput = value.Get<Vector2>();
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer);
    }

    #region detectenemyCollision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //als een collision de tag enemy heeft wordt enemy hit true
        if (collision.gameObject.tag == "Enemy")
        {
            enemyHit = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //als gameobject enemy niet raakt wordt enemyHit false
        if (collision.gameObject.tag == "Enemy")
        {
            enemyHit = false;
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {   //als collider met tag save1 heeft = save true
        if (other.gameObject.tag == "Save1")
        {
            hasReachedSavePoint = true;
            //update speler positie wanneer savepoint is bereikt
            startLocation = GameObject.FindGameObjectWithTag("Save1").transform.position;
        }
        if (other.gameObject.tag == "FinishLocationLvl1")
        {

            SceneManager.LoadScene("Level2");
        }
        if (other.gameObject.tag == "FinishLocationLvl2")
        {

            SceneManager.LoadScene("Level3");
        }
        if (other.gameObject.tag == "FinishLocationLvl3")
        {
            SceneManager.LoadScene("endMenu");
        }
    }

    void RespawnAtSavePoint()
    {
        body.position = startLocation; //update player current position and store it in startLocation field
        enemyHit = false; //reset enemyhit status
        isFalling = false;
    }

    void RespawnAtStart()
    {
        body.position = startLocation;
        enemyHit = false;
        hasReachedSavePoint = false;
        isFalling = false;
    }

    void CheckIfFalling()
    {
        if(transform.position.y < fallPosition) 
        {
            isFalling = true;
        } else
        {
            isFalling = false;
        }

    }

    void HandleFalling()
    {
        if(isFalling)
        {
            if (!hasReachedSavePoint)
            {
                RespawnAtStart();
            } else 
            {
                RespawnAtSavePoint();
            }
        }
    }
}




