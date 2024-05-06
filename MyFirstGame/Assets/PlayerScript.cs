using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        Vector2 playerVelocity = new Vector2(moveInput.x * moveInput.y, body.velocity.y);
    }

    void OnMove(InputValue value)
    {
    //var moveInput zegt of er op L of R wordt geklikt
    moveInput = value.Get<Vector2>();
    }
}




