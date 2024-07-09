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

public class GrabController : NetworkBehaviour
{
    public Transform grabDetect;
    public Transform objectHolder;
    public Transform umbrellaPosition;
    public float rayDist;

    private Collider2D[] playerColliders;
    private GameObject grabbedObject = null;

    private NetworkVariable<ulong> grabbedObjectId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        playerColliders = GetComponents<Collider2D>(); // Assumes the player has a Collider2D component
        grabbedObjectId.OnValueChanged += OnGrabbedObjectChanged;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (grabbedObject == null)
        {
            RaycastHit2D grabCheck = Physics2D.Raycast(grabDetect.position, Vector2.right * transform.localScale.x, rayDist);
            if (grabCheck.collider != null && (grabCheck.collider.CompareTag("Rock") || grabCheck.collider.CompareTag("Umbrella")))
            {
                if (Input.GetButton("Grab"))
                {
                    grabbedObject = grabCheck.collider.gameObject;
                    RequestOwnershipServerRpc(grabbedObject.GetComponent<NetworkObject>().NetworkObjectId);
                    grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                    IgnoreAllCollisions(grabbedObject, true);
                    UpdateGrabbedObjectPosition(grabbedObject);
                    UpdateGrabbedObjectPositionClientRpc(grabbedObject.GetComponent<NetworkObject>().NetworkObjectId, objectHolder.position);
                }
            }
        }
        else
        {
            UpdateGrabbedObjectPosition(grabbedObject);

            if (!Input.GetButton("Grab"))
            {
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                IgnoreAllCollisions(grabbedObject, false);
                ThrowObject(grabbedObject);
                grabbedObjectId.Value = 0;
                grabbedObject = null;
            }
        }
    }

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

    private void FixedUpdate()
    {
        if (grabbedObjectId.Value != 0 && grabbedObject != null)
        {
            UpdateGrabbedObjectPosition(grabbedObject);
        }
    }

    private void UpdateGrabbedObjectPosition(GameObject grabbedObject)
    {
        if (grabbedObject != null)
        {
            Transform targetPosition = grabbedObject.CompareTag("Umbrella") ? umbrellaPosition : objectHolder;
            grabbedObject.transform.position = targetPosition.position;
        }
    }

    [ClientRpc]
    private void UpdateGrabbedObjectPositionClientRpc(ulong objectId, Vector3 position)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
        {
            netObj.transform.position = position;
        }
    }

    [ServerRpc]
    private void RequestOwnershipServerRpc(ulong objectId, ServerRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
        {
            netObj.ChangeOwnership(rpcParams.Receive.SenderClientId);
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





