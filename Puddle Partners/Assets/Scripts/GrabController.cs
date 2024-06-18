using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    public Transform grabDetect;
    public Transform objectHolder;
    public float rayDist;
    private GameObject grabbedObject;
    private Collider2D playerCollider;
    private float oldPosition;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>(); // Assumes the player has a Collider2D component
    }

    void Update()
    {
        if (grabbedObject == null)
        {
            // Only check for objects to grab if we are not already holding one
            RaycastHit2D grabCheck = Physics2D.Raycast(grabDetect.position, Vector2.right * transform.localScale.x, rayDist);
            if (grabCheck.collider != null && grabCheck.collider.CompareTag("Rock"))
            {
                if (Input.GetButton("Grab"))
                {
                    // Grab the object
                    grabbedObject = grabCheck.collider.gameObject;
                    Collider2D grabbedCollider = grabbedObject.GetComponent<Collider2D>();
                    Physics2D.IgnoreCollision(playerCollider, grabbedCollider, true); // Ignore collisions with the grabbed object
                    grabbedObject.transform.parent = objectHolder;
                    grabbedObject.transform.position = objectHolder.position;
                    grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                }
            }
        }
        else
        {
            if (!Input.GetButton("Grab"))
            {
                // Release the object
                Collider2D grabbedCollider = grabbedObject.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(playerCollider, grabbedCollider, false); // Re-enable collisions with the released object
                grabbedObject.transform.parent = null;
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                throwObject(grabbedObject);
                grabbedObject = null;
            }
        }
    }
    private void throwObject(GameObject grabbedObject)
    {
        if (transform.GetComponent<Rigidbody2D>().velocity.magnitude > 0) 
        {
            if (transform.localScale.x > 0)
            {
                grabbedObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(400f, 100f));
            }
            else
            {
                grabbedObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-400f, 100f));
            }
        }
    }
}


