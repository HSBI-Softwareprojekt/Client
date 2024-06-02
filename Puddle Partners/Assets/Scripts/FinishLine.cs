using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private bool finishedLevel = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !finishedLevel)
        {
            finishedLevel = true;
            print("Finished the first Level!");
        }
    }
}
