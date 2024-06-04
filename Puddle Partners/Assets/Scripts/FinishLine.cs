using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class FinishLine : MonoBehaviour
{
    private GameObject[] player;
    private bool finishedLevel = false;
    private GameObject playerCanvas;
    

    private void Start()
    {
        //playerSpawner = GameObject.FindGameObjectWithTag("Finish");
        player = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(player[0]);


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !finishedLevel)
        {
            finishedLevel = true;
            showScoreboardMenu();
            UnlockNewLevel();
            print("Finished the first Level!");
        }
    }

    void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }

    void showScoreboardMenu()
    {
        foreach (GameObject p in player)
        {
            playerCanvas = p.transform.GetChild(4).gameObject;
            Debug.Log(playerCanvas);
            playerCanvas.SetActive(true);

        }
    }
}
