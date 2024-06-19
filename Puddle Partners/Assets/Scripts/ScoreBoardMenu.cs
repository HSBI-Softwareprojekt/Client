using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreBoardMenu : NetworkBehaviour
{
    private PlayerSpwaner playerSpawner;
    private int nextLevelId;
    private void Start()
    {
        nextLevelId = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Log(nextLevelId);
        playerSpawner = FindObjectOfType<PlayerSpwaner>();

    }
    //If Host, go back to the lobby. If Client, go back in the main menu.

    //TODO: client case
    public void menuButton()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.SceneManager.LoadScene("LoginRegister", LoadSceneMode.Single);
            Destroy(playerSpawner.gameObject);
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }
    //TODO: For the user experience, add a case where a client gets feedback, for not being able to use the button
    public void nextLevelButton()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Level_" + nextLevelId.ToString(), LoadSceneMode.Single);
        }
    }
}

