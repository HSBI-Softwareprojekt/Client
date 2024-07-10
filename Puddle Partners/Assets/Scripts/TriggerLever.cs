using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Used when a lever is pulled
public class TriggerLever : NetworkBehaviour
{
    // check if the lever is pulled or not
    public bool leverOn;
    // how the lever looks like if not pulled
    public Sprite leverOffSprite;
    // how the lever looks like if pulled
    public Sprite leverOnSprite;
    private SpriteRenderer spriteRenderer;
    //the object that is manipulated through the lever
    public GameObject obj;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = leverOffSprite;
    }

    // pulling the lever
    public void PullLever()
    {
        FinishLine finishLine = obj.GetComponent<FinishLine>();
        SpriteRenderer spriteFinish = obj.GetComponent<SpriteRenderer>();
        bool newState = !obj.activeSelf;

        // check whether it manipulates a finish line or another Object and uses logic accordingly
        if (finishLine != null)
        {
            if (!leverOn)
            {
                leverOn = true;
                spriteRenderer.sprite = leverOnSprite;
                finishLine.isAvailable = true;
                spriteFinish.enabled = false;
                Debug.Log("Lever pulled open");
            }
            else
            {
                leverOn = false;
                spriteRenderer.sprite = leverOffSprite;
                finishLine.isAvailable = false;
                spriteFinish.enabled = true;
                Debug.Log("Lever pulled closed");
            }
        }
        else
        {
            if (NetworkManager.IsHost)
            {
                PullLeverServerRpc(newState);
            }

        }

    }

    // Function for the Server to execute to manipulate an Object through the lever
    [ServerRpc]
    private void PullLeverServerRpc(bool newState)
    {
        obj.SetActive(newState);
        PullLeverClientRpc(newState);
    }

    // Function that executes on all the client to manipulate an Object if the Server did so
    [ClientRpc]
    private void PullLeverClientRpc(bool newState)
    {
        obj.SetActive(newState);
    }
}
