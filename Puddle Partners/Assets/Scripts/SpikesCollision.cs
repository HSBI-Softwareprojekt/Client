using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reset the Players Position, if a Player collides with Spikes
public class SpikesCollision : MonoBehaviour
{
    // Reset the Players Position on collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.gameObject.transform.localPosition = new Vector2 (0, 0);
        }
    }
}
