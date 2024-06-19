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

public class login : MonoBehaviour
{

    public GameObject loginMenue;
    public GameObject mainMenue;
    public TMP_InputField username;
    public TMP_InputField password;
    public GameObject errorPanel;
    public TMP_Text errorMsg;

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
        public int id;
        public string name;

        public int getId() { return id; }
        public string getName() { return name; }
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
        StreamReader reader = new StreamReader(Application.dataPath+"/config/server.pudl");
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

    private void LoginSuccessfull(int id, string name)
    {
        PlayerPrefs.SetString("LoginID", id.ToString());
        PlayerPrefs.SetString("LoginName", name);
        //SceneManager.LoadScene(1);
        /*loginMenue.SetActive(false);
        mainMenue.SetActive(true);*/
    }

    public void RequestLogin()
    {
        WebRequest request = WebRequest.Create("http://" + HttpServer() + "/puddle_partners/login.php");
        ASCIIEncoding encoding = new ASCIIEncoding();
        string postData = "USERNAME=" + username.text + "&PASSWORD=" + password.text;
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
            Data playerData = JsonConvert.DeserializeObject<Data>(responseText);
            if (playerData.getId() != 0)
            {
                LoginSuccessfull(playerData.getId(), playerData.getName());
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

    public void QuitGame()
    {
        Application.Quit();
    }

    // Für HTTPS von Unity
    /*public void RequestLogin()
    {
        StartCoroutine(Request());
    }

    IEnumerator Request()
    {
        WWWForm data = new WWWForm();
        data.AddField("USERNAME", username.text);
        data.AddField("PASSWORD", password.text);
        using UnityWebRequest uwr = UnityWebRequest.Post("https://192.168.178.31/puddle_partners/login.php", data);
        yield return uwr.SendWebRequest();
        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            var response = uwr.downloadHandler.text;
        }
    }*/

}