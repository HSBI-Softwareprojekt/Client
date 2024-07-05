using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCheck : MonoBehaviour
{
    public GameObject obj;
    private bool wasUsed = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Rock") && obj.activeSelf == true)
        {
            obj.SetActive(false);
            wasUsed = true;
            Debug.Log("Was used.");
        }
        else if(collision.gameObject.CompareTag("Rock") && obj.activeSelf == false)
        {
            obj.SetActive(true);
            wasUsed = true;
            Debug.Log("Was used.");
        }
    }
}
