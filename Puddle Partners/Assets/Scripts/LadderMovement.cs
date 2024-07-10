using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For Player Movements on Ladders
public class LadderMovement : MonoBehaviour
{
    private float vertical;
    private float climbSpeed = 8f;
    private bool isLadder;
    private bool isClimbing;

    // The Players gravity Component
    [SerializeField] private Rigidbody2D rb;

    // Checks if a Player is Climbing or not
    void Update()
    {
        vertical = Input.GetAxis("Vertical");
        if (Input.GetButton("Jump"))
        {
            isClimbing = false;
        }
        else if (isLadder && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
        }
    }

    private void FixedUpdate()
    {
        if(isClimbing)
        {
            // disables Gravity, if the Player is climbing
            rb.gravityScale = 0f;
            // Updates the Position in the Direction of the Movement
            rb.velocity = new Vector2(rb.velocity.x, vertical * climbSpeed);
        }
        else
        {
            // Reenables gravity
            rb.gravityScale = 3f;
        }
    }

    // Checks if the Player is on a Ladder
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }

    // Checks if the Player is not on a Ladder anymore
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
    }
}
