using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    public Transform grabDetect;
    public Transform objectHolder;
    public Transform umbrellaPosition;
    public float rayDist;
    private GameObject grabbedObject;
    private Collider2D[] playerColliders;
    private float oldPosition;
//0,06 0.05

    void Start()
    {
        playerColliders = GetComponents<Collider2D>(); // Assumes the player has a Collider2D component
    }

    void Update()
    {
        if (grabbedObject == null)
        {
            RaycastHit2D grabCheck = Physics2D.Raycast(grabDetect.position, Vector2.right * transform.localScale.x, rayDist);
            if (grabCheck.collider != null && (grabCheck.collider.CompareTag("Rock") || grabCheck.collider.CompareTag("Umbrella")))
            {
                if (Input.GetButton("Grab"))
                {
                    grabbedObject = grabCheck.collider.gameObject;
                    IgnoreAllCollisions(grabbedObject, true);
                    if (grabbedObject.CompareTag("Umbrella"))
                    {
                        grabbedObject.transform.SetParent(umbrellaPosition);
                        grabbedObject.transform.position = umbrellaPosition.position;
                    }
                    else
                    {
                        grabbedObject.transform.SetParent(objectHolder);
                        grabbedObject.transform.position = objectHolder.position;
                    }
                    grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                }
            }
        }
        else
        {
            if (!Input.GetButton("Grab"))
            {
                IgnoreAllCollisions(grabbedObject, false);
                grabbedObject.transform.SetParent(null);
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                throwObject(grabbedObject);
                grabbedObject = null;
            }
        }
    }

    private void IgnoreAllCollisions(GameObject obj, bool ignore)
    {
        Collider2D[] objectColliders = obj.GetComponents<Collider2D>();
        foreach (Collider2D playerCollider in playerColliders)
        {
            foreach (Collider2D objCollider in objectColliders)
            {
                Physics2D.IgnoreCollision(playerCollider, objCollider, ignore);
            }
        }
    }

    private void throwObject(GameObject obj)
    {
        if (obj.CompareTag("Umbrella"))
        {
            return;
        }

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        Vector2 forceDirection = transform.localScale.x > 0 ? new Vector2(400f, 100f) : new Vector2(-400f, 100f);
        rb.AddForce(forceDirection);
    }
}


