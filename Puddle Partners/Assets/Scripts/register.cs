using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System.Net;
using System;
using Newtonsoft.Json;

public class register : MonoBehaviour
{

    public TMP_InputField mail;
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_InputField password_wd;
    public GameObject errorPanel;
    public TMP_Text errorMsg;
    public GameObject loginScreen;
    public GameObject registerScreen;

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
        public int ok;

        public int getOk() { return ok; }
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

    private void RegisterSuccessfull()
    {
        registerScreen.SetActive(false);
        loginScreen.SetActive(true);
    }

    public void RequestRegister()
    {
        WebRequest request = WebRequest.Create("http://" + HttpServer() + "/puddle_partners/register.php");
        ASCIIEncoding encoding = new ASCIIEncoding();
        string postData = "EMAIL=" + mail.text + "&USERNAME=" + username.text + "&PASSWORD=" + password.text + "&PASSWORD_WD=" + password_wd.text;
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
            Data ok = JsonConvert.DeserializeObject<Data>(responseText);
            if (ok.getOk() != 0)
            {
                RegisterSuccessfull();
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

}
