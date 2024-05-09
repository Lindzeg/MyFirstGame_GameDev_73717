using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    //serialize zodat je de waarde in unity kan veranderen
    //kan niet door scripts worden aangespast
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private float jumpingPower = 8f;
    private Vector2 startLocation = Vector3.zero;

    //Vector 2 voor x en y input (vector3 is xyz)
    Vector2 moveInput;

    //body Player
    Rigidbody2D body;
    //used in method oncollision
    bool enemyHit = false;
    bool hasReachedSavePoint = false; //to track if savepoint is reached

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        startLocation = body.position;
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
        //bepaald welke kant player beweegd op basis van moveinput var
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, body.velocity.y);

        //als de speler op W klikt en de grond word geraakt, dan springt player
        if (moveInput.y > 0 && IsGrounded())
        {
            playerVelocity = new Vector2(moveInput.x, jumpingPower);

        }

        body.velocity = playerVelocity;     

        //controleerd of player beweegd
        bool playerMoves = Mathf.Abs(playerVelocity.x) > Mathf.Epsilon;
        if (playerMoves)
        {
            //draait speler op basis hoe speler beweegd
            transform.localScale = new Vector2(Mathf.Sign(body.velocity.x), transform.localScale.y);
        }
        #endregion

        if(enemyHit) 
        {
            Debug.Log("Geraakt");

            if(hasReachedSavePoint)
            {
                RespawnAtSavePoint();
            }
            else
            {
                RespawnAtStart();
            }
        }
        
    }

    void OnMove(InputValue value)
    {
        //var moveInput zegt of er op L of R wordt geklikt
        moveInput = value.Get<Vector2>();
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.6f, groundLayer);
    }

    #region detectenemyCollision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //als de andere collision de tag enemy heeft wordt enemy hit true
        if (collision.gameObject.tag == "Enemy")
        {
            enemyHit = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            enemyHit = false;
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {   //als andere collider de tag enemy heeft word enemyhit true
        if(other.gameObject.tag == "Save1")
        {
            hasReachedSavePoint = true;
            //waar player daadwerkelijk begint als spel begint
            startLocation = GameObject.FindGameObjectWithTag("Save1").transform.position;
            Debug.Log("Save");
            Debug.Log("startLocation set to: " + startLocation); //startLocation controleren
        }
    }

    void RespawnAtSavePoint()
    {
        body.position = startLocation; //teleport to savepoint
        enemyHit = false; //reset enemyhit status
    }

    void RespawnAtStart()
    {
        body.position = startLocation;
        enemyHit = false;
        hasReachedSavePoint= false;
    }
}




