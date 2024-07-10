using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

// Manualy Spawns the Player into the Level
public class PlayerSpwaner : NetworkBehaviour
{
    // The Player Object
    [SerializeField]
    private GameObject Player;
    private void Start()
    {
        // Let the Spawner change Levels with the Player
        DontDestroyOnLoad(this.gameObject);
    }

    // Let the Networkmanager listen to changes of the Scene
    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += Sceneloaded;
    }

    // Let Players Spawn, if they are playing a Level
    private void Sceneloaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (NetworkManager.IsHost && sceneName != "LoginRegister")
        {
            foreach(ulong id in clientsCompleted)
            {
                GameObject player = Instantiate(Player);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
            }
        }
    }
}
