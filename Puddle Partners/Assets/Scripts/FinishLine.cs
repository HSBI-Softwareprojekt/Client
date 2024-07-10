using Unity.Netcode;
using UnityEngine;

// The Finishline that marks the end of a Level
public class FinishLine : MonoBehaviour
{
    // The Connected Players
    private GameObject[] player;
    // Checks if the Finishline can be crossed
    public bool isAvailable;
    // A Finishline Object
    public GameObject finish;

    // Find all the Players at the Start of the Level
    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
    }

    // Checks if the Player crosses the Finishline
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isAvailable)
        {
            // Script Component for handling Gamelogic for finishing a Game
            FinishLevel finishLevel = finish.GetComponent<FinishLevel>();
            // Call a Function for finishing a Level on the Server
            finishLevel.FinishedServerRpc();
        }
    }

}