using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Checks if a Rock is being thrown into a specific Position and manipulates an Object accordingly
public class GoalCheck : NetworkBehaviour
{
    // Object to be manipulated
    public GameObject obj;
    // Checks if the goal was already used, so code doesnt execute twice
    private bool wasUsed = false;

    // Checks if the Rock makes Contact with a Goal
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Rock") && !wasUsed)
        {
            wasUsed = true;
            // Only the server should handle the state change
            if (NetworkManager.IsHost)
            {
                Debug.Log("Start goal event");
                bool newState = !obj.activeSelf;
                // Change Object State
                SetObjectActiveServerRpc(newState);
            }
        }
    }

    // Change Object State on the Server
    [ServerRpc]
    private void SetObjectActiveServerRpc(bool newState)
    {
        obj.SetActive(newState);
        // Synchronize the state change with all clients
        SetObjectActiveClientRpc(newState);
    }

    // Change the Object State on all Clients
    [ClientRpc]
    private void SetObjectActiveClientRpc(bool newState)
    {
        obj.SetActive(newState);
        // Ensure, that the goal was used on all Clients
        wasUsed = true;
        Debug.Log("Was used.");
    }
}
