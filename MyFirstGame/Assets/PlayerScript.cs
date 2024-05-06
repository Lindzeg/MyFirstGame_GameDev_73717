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

    //Vector 2 voor x en y input (vector3 is xyz)
    Vector2 moveInput;

    //body Player
    Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //bepaald welke kant player beweegd op basis van moveinput var
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, body.velocity.y);
        body.velocity = playerVelocity;

        //controleerd of player beweegd
        bool playerMoves = Mathf.Abs(playerVelocity.x) > Mathf.Epsilon;
        if (playerMoves)
        {
            //draait speler op basis hoe speler beweegd
            transform.localScale = new Vector2(Mathf.Sign(body.velocity.x), transform.localScale.y);
        }

        //reset hele scene
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene("LevelOne");
        }
    }

    void OnMove(InputValue value)
    {
        //var moveInput zegt of er op L of R wordt geklikt
        moveInput = value.Get<Vector2>();
    }
}




