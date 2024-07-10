using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Events;

// Checks if a Player is in Range for certain Interactions
public class Interactable : MonoBehaviour
{
    // Checks if a Player is in Range
    public bool isInRange;
    // The Key to trigger Interactions
    public KeyCode interactKey;
    // The Event that triggers through the Interaction
    public UnityEvent interaction; 

    // Check if a Player is in Range every frame
    void Update()
    {
        if (isInRange)
        {
            // Triggers Interaction through input
            if(Input.GetKeyDown(interactKey))
            {
                interaction.Invoke();
            }
        }
    }

    // Checks if the Player steps in Range
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
            Debug.Log("Player is in Range");
        }
    }

    // Checks if the PLayer steps out of Range
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = false;
            Debug.Log("Player is out of Range");
        }
    }
}
