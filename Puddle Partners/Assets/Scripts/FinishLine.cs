using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public bool isAvailable;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && isAvailable)
        {
            print("Finished the first Level!");
        }
    }
}
