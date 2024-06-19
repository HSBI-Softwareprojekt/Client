using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Net;
using System;
using System.Text;
using UnityEngine.UI;
using Newtonsoft.Json;
using Unity.Netcode;

public class FinishLine : MonoBehaviour
{
    public GameObject errorPanel;
    public TMP_Text errorMsg;
    private GameObject[] player;
    private bool finishedLevel = false;
    private GameObject playerCanvas;
    public bool isAvailable;

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    [System.Serializable]
    public class LevelData
    {
        public int state;

        public int getState() { return state; }
    }

    [System.Serializable]
    public class ScoreData
    {
        ArrayList data;

        public ArrayList getScores()
        {
            return data;
        }
    }

    [System.Serializable]
    public class Error
    {
        public ArrayList error;
        public ArrayList getError() { return error; }
    }

    private string HttpServer()
    {
        string url = "";
        StreamReader reader = new StreamReader(Application.dataPath + "/config/server.pudl");
        url = reader.ReadToEnd();
        reader.Close();
        return url;
    }

    private void Start()
    {
        //playerSpawner = GameObject.FindGameObjectWithTag("Finish");
        player = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(player[0]);


    }

    private void ErrorMsg(ArrayList msg)
    {
        string showMsg = "";
        for (int i = 0; i < msg.Count; i++)
        {
            showMsg += msg[i] + "\n\n";
        }
        errorMsg.text = showMsg;
        errorPanel.SetActive(true);
    }

    public void ErrorMsgOK()
    {
        errorPanel.SetActive(false);
        errorMsg.text = "";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !finishedLevel && isAvailable)
        {
            finishedLevel = true;
            print("Finished Level!");
            setLevel();          
            showScoreboardMenu();
            //UnlockNewLevel();
        }
    }

    private void setLevel()
    {
        WebRequest request = WebRequest.Create("http://" + HttpServer() + "/puddle_partners/set_level.php");
        ASCIIEncoding encoding = new ASCIIEncoding();
        string postData = "ID=" + PlayerPrefs.GetString("LoginID") + "&LEVEL=" + PlayerPrefs.GetString("Level");
        byte[] data = encoding.GetBytes(postData);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = data.Length;
        Stream stream = request.GetRequestStream();
        stream.Write(data, 0, data.Length);
        stream.Close();
        WebResponse response = request.GetResponse();
        stream = response.GetResponseStream();
        StreamReader reader = new StreamReader(stream);
        string responseText = reader.ReadToEnd();
        reader.Close();
        stream.Close();
        try
        {
            LevelData playerData = JsonConvert.DeserializeObject<LevelData>(responseText);
            if (playerData.getState() != 0)
            {
                //getScores();
                UnlockNewLevel();
            }
            else
            {
                Error error = JsonConvert.DeserializeObject<Error>(responseText);
                ErrorMsg(error.getError());
            }
        }
        catch (Exception)
        {
            ArrayList err = new ArrayList();
            err.Add("Kein gültiges Rückgabeformat vom Webserver");
            ErrorMsg(err);
        }
    }

    private void getScores()
    {
        WebRequest request = WebRequest.Create("http://" + HttpServer() + "/puddle_partners/scoreboard.php");
        ASCIIEncoding encoding = new ASCIIEncoding();
        string postData = "HOST=" + PlayerPrefs.GetString("LoginID") + "&CLIENT=" + PlayerPrefs.GetString("PartnerID") + "&TIME=";
        byte[] data = encoding.GetBytes(postData);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = data.Length;
        Stream stream = request.GetRequestStream();
        stream.Write(data, 0, data.Length);
        stream.Close();
        WebResponse response = request.GetResponse();
        stream = response.GetResponseStream();
        StreamReader reader = new StreamReader(stream);
        string responseText = reader.ReadToEnd();
        reader.Close();
        stream.Close();
        try
        {
            ScoreData playerData = JsonConvert.DeserializeObject<ScoreData>(responseText);
            if ((int)playerData.getScores()[0] != 0)
            {
                
            }
            else
            {
                Error error = JsonConvert.DeserializeObject<Error>(responseText);
                ErrorMsg(error.getError());
            }
        }
        catch (Exception)
        {
            ArrayList err = new ArrayList();
            err.Add("Kein gültiges Rückgabeformat vom Webserver");
            ErrorMsg(err);
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