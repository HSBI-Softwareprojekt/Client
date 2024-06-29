using Unity.Netcode;
using UnityEngine;

public class FinishLine : MonoBehaviour
{

    private GameObject[] player;
    public bool isAvailable;
    public GameObject finish;

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isAvailable)
        {
            FinishLevel finishLevel = finish.GetComponent<FinishLevel>();
            finishLevel.FinishedServerRpc();
        }
    }

}