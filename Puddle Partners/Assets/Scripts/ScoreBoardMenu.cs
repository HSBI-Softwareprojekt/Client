using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// The Scoreboard Menu options, when a Level finishes
public class ScoreBoardMenu : NetworkBehaviour
{
    // The Object, that handles the spawning of Players
    private PlayerSpwaner playerSpawner;
    // the id of the next Level
    private int nextLevelId;

    private void Start()
    {
        // Set the new Level id to the one, after the current one
        nextLevelId = SceneManager.GetActiveScene().buildIndex + 1;
        playerSpawner = FindObjectOfType<PlayerSpwaner>();

    }
    //If Host, go back to the lobby. If Client, go back in the main menu.
    public void menuButton()
    {
        if (IsHost)
        {
            // Shutdown the Connection
            NetworkManager.Singleton.Shutdown();
            // go back to the main menu
            NetworkManager.Singleton.SceneManager.LoadScene("LoginRegister", LoadSceneMode.Single);
            // Destroy the PlayerSpawner in order to avoid duplicates
            Destroy(playerSpawner.gameObject);
            // Destroy the NetworkManager in order to avoid duplicates
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }
    
    // Go to the next Level if a Button is pressed
    public void nextLevelButton()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Level_" + nextLevelId.ToString(), LoadSceneMode.Single);
        }
    }
}

