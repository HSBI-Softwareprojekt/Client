using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode.Components;
using UnityEngine.UIElements;

// Script for grabbing Objects and throwing them
public class GrabController : NetworkBehaviour
{
    // Position to detect Objects that can be grabbed
    public Transform grabDetect;
    // Position where the Object will be Placed if grabbed
    public Transform objectHolder;
    // Specific Position for Umbrellas if grabbed
    public Transform umbrellaPosition;
    // Distance for detecting Objects
    public float rayDist;

    // All the Collider Hitboxes a Player has
    private Collider2D[] playerColliders;
    // Object to be grabbed
    private GameObject grabbedObject = null;
    // Id of the grabbed that will be used to synchronize the Position for all the Clients
    private NetworkVariable<ulong> grabbedObjectId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        playerColliders = GetComponents<Collider2D>();
        // Get the Id of the grabbed Object
        grabbedObjectId.OnValueChanged += OnGrabbedObjectChanged;
    }

    // Check every frame if a Object can be grabbed, or update the grabbed Objects Position
    private void Update()
    {
        // if it's not the Clients Owner, dont execute code
        if (!IsOwner)
        {
            return;
        }

        // If the Player is currently not holding an Object
        if (grabbedObject == null)
        {
            // The Object that was detected in the grabable Area
            RaycastHit2D grabCheck = Physics2D.Raycast(grabDetect.position, Vector2.right * transform.localScale.x, rayDist);
            // Check for hitbox Information and if its an Objects that can be grabbed 
            if (grabCheck.collider != null && (grabCheck.collider.CompareTag("Rock") || grabCheck.collider.CompareTag("Umbrella")))
            {
                if (Input.GetButton("Grab"))
                {
                    // Set the grabbed Object
                    grabbedObject = grabCheck.collider.gameObject;
                    // Allow the Clients to manipulate the Objects Position
                    RequestOwnershipServerRpc(grabbedObject.GetComponent<NetworkObject>().NetworkObjectId);
                    // Disable gravity for the grabbed Object
                    grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                    // Disable collision logic for the grabbed Object
                    IgnoreAllCollisions(grabbedObject, true);
                    // Update the grabbed Position 
                    UpdateGrabbedObjectPosition(grabbedObject);
                    // Update the Grabbed Objects State on all Clients
                    UpdateGrabbedObjectPositionClientRpc(grabbedObject.GetComponent<NetworkObject>().NetworkObjectId, objectHolder.position);
                }
            }
        }
        else
        {
            // Update the grabbed Object Position
            UpdateGrabbedObjectPosition(grabbedObject);
            // The Objects gets released
            if (!Input.GetButton("Grab"))
            {
                // reenable gravity for the Object
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                // reenable collision for the Object
                IgnoreAllCollisions(grabbedObject, false);
                // Throw the Objects
                ThrowObject(grabbedObject);
                // Reset the Object and Id
                grabbedObjectId.Value = 0;
                grabbedObject = null;
            }
        }
    }

    // Get the Id of the new Grabbed Object, or set it to zero
    private void OnGrabbedObjectChanged(ulong previousValue, ulong newValue)
    {
        if (newValue == 0)
        {
            if (grabbedObject != null)
            {
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                IgnoreAllCollisions(grabbedObject, false);
                ThrowObject(grabbedObject);
                grabbedObject = null;
            }
        }
        else
        {
            grabbedObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newValue].gameObject;
            grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
            IgnoreAllCollisions(grabbedObject, true);
        }
    }

    // Update the grabbed Objects Position locally, if one is present
    private void FixedUpdate()
    {
        if (grabbedObjectId.Value != 0 && grabbedObject != null)
        {
            UpdateGrabbedObjectPosition(grabbedObject);
        }
    }

    // Updates the local Position of the grabbed Object
    private void UpdateGrabbedObjectPosition(GameObject grabbedObject)
    {
        if (grabbedObject != null)
        {
            // Sets the Position to the designated position, whether it's an Umbrella or a different Object
            Transform targetPosition = grabbedObject.CompareTag("Umbrella") ? umbrellaPosition : objectHolder;
            grabbedObject.transform.position = targetPosition.position;
        }
    }

    // Update the Objects Position on all Clients
    [ClientRpc]
    private void UpdateGrabbedObjectPositionClientRpc(ulong objectId, Vector3 position)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
        {
            netObj.transform.position = position;
        }
    }

    // Changes the Ownership of the grabbed Object, so the Client can manipulate the Position
    [ServerRpc]
    private void RequestOwnershipServerRpc(ulong objectId, ServerRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
        {
            netObj.ChangeOwnership(rpcParams.Receive.SenderClientId);
        }
    }
    
    // Disable Collision Hitbox for all the Clients
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

    // throws the Object, if it's not an Umbrella
    private void ThrowObject(GameObject obj)
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





