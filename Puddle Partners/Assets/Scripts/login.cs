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

public class login : MonoBehaviour
{

    public TMP_InputField username;
    public TMP_InputField password;
    public GameObject errorPanel;
    public TMP_Text errorMsg;

    private string HttpServer()
    {
        string url = "";
        StreamReader reader = new StreamReader(Application.dataPath+"/config/server.pudl");
        url = reader.ReadToEnd();
        reader.Close();
        return url;
    }

    private ArrayList HttpParser(string text)
    {
        string sep = "<!=!>";
        string[] sp = text.Split(sep, StringSplitOptions.None);
        ArrayList data = new ArrayList(sp);
        return data;
    }

    private void ErrorMsg(ArrayList msg)
    {
        string showMsg = "";
        for (int i = 1; i < msg.Count; i++)
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

    private void LoginSuccessfull(int id)
    {
        PlayerPrefs.SetString("LoginID", id.ToString());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RequestLogin()
    {
        WebRequest request = WebRequest.Create("http://"+ HttpServer() + "/puddle_partners/login.php");
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
        ArrayList responseData = HttpParser(responseText);
        if (responseData[0].ToString() == "ERROR")
        {
            ErrorMsg(responseData);
        } else if (responseData[0].ToString() == "DATA")
        {
            int id;
            if (Int32.TryParse(responseData[1].ToString(), out id))
            {
                LoginSuccessfull(id);
            } else
            {
                ArrayList errorData = new ArrayList();
                errorData.Add("ID konnte nicht konvertiert werden");
                ErrorMsg(errorData);
            }
        } else
        {
            ArrayList errorData = new ArrayList();
            errorData.Add("Server Rückgabe Fehler");
            ErrorMsg(errorData);
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
            Debug.Log(response);
        }
    }*/

}