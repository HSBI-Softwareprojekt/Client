using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLever : MonoBehaviour
{

    public bool leverOn;
    public Sprite leverOffSprite;
    public Sprite leverOnSprite;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = leverOffSprite;
    }

    public void PullLever(GameObject finish)
    {
        FinishLine finishLine = finish.GetComponent<FinishLine>();
        SpriteRenderer spriteFinish = finish.GetComponent<SpriteRenderer>();

        
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

    }

}
