using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System.Net;
using System;

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

    private string HttpServer()
    {
        string url = "";
        StreamReader reader = new StreamReader(Application.dataPath + "/config/server.pudl");
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
        ArrayList responseData = HttpParser(responseText);
        if (responseData[0].ToString() == "ERROR")
        {
            ErrorMsg(responseData);
        }
        else if (responseData[0].ToString() == "DATA")
        {
            if (responseData[1].ToString() == "TRUE")
            {
                RegisterSuccessfull();
            }
            else
            {
                ArrayList errorData = new ArrayList();
                errorData.Add("Registrierung konnte nicht abgeschlossen werden");
                ErrorMsg(errorData);
            }
        }
        else
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

}
