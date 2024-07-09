using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GoalCheck : NetworkBehaviour
{
    public GameObject obj;
    private bool wasUsed = false;

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
                SetObjectActiveServerRpc(newState);
            }
        }
    }

    [ServerRpc]
    private void SetObjectActiveServerRpc(bool newState)
    {
        // Update the object state on the server
        obj.SetActive(newState);


        // Synchronize the state change with all clients
        SetObjectActiveClientRpc(newState);
    }

    [ClientRpc]
    private void SetObjectActiveClientRpc(bool newState)
    {
        // Update the object state on all clients
        obj.SetActive(newState);
        wasUsed = true;
        Debug.Log("Was used.");
    }
}
