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

public class SelectLevel : MonoBehaviour
{

    public GameObject forwards;
    public GameObject backwards;
    public GameObject errorPanel;
    public GameObject start;
    public TMP_Text errorMsg;
    public TMP_Text levelText;
    private int level = 1;
    private Data playerData;
    private int max_level = 4;

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
    public class Data
    {
        public int[] level;

        public int[] getLevel() { return level; }

        public void setDefault()
        {
            level[0] = 0;
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

    private void enableButtonOptions()
    {
        start.SetActive(true);
        forwards.SetActive(true);
    }

    public void switchLevelUp()
    {
        level++;
        bool is_active = false;
        foreach(int lvl in playerData.getLevel())
        {
            if (lvl == (level-1))
            {
                is_active = true;
                break;
            }
        }
        levelText.text = "Level " + level;
        if(!is_active)
        {
            start.SetActive(false);
        }
        else
        {
            start.SetActive(true);
        }
        backwards.SetActive(true);
        if(level == max_level)
        {
            forwards.SetActive(false);
        }
    }

    public void switchLevelDown()
    {
        level--;
        bool is_active = false;
        if (level != 1)
        {
            foreach (int lvl in playerData.getLevel())
            {
                if (lvl == (level - 1))
                {
                    is_active = true;
                    break;
                }
            }
        } else
        {
            is_active = true;
        }
        levelText.text = "Level " + level;
        if (!is_active)
        {
            start.SetActive(false);
        }
        else
        {
            start.SetActive(true);
        }
        forwards.SetActive(true);
        if (level == 1)
        {
            backwards.SetActive(false);
        }
    }

    public void startLevel()
    {
        PlayerPrefs.SetString("Level", level.ToString());
        NetworkManager.Singleton.SceneManager.LoadScene("Level_"+level.ToString() + "_test", LoadSceneMode.Single);
    }

    void OnEnable()
    {
        WebRequest request = WebRequest.Create("http://" + HttpServer() + "/puddle_partners/get_level.php");
        ASCIIEncoding encoding = new ASCIIEncoding();
        string postData = "ID="+ PlayerPrefs.GetString("LoginID");
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
            playerData = JsonConvert.DeserializeObject<Data>(responseText);
            if (playerData.getLevel().Length != 0 && playerData.getLevel()[0] != 0)
            {
                if(playerData.getLevel().Length == 1 && playerData.getLevel()[0] == -1)
                {
                    playerData.setDefault();
                } 
                enableButtonOptions();
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
            err.Add("Kein g�ltiges R�ckgabeformat vom Webserver");
            ErrorMsg(err);
        }
    }



}