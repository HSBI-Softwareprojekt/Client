using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class login : MonoBehaviour
{

    public TMP_InputField username;
    public TMP_InputField password;
 
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RequestLogin()
    {
        print(username.text);
        print(password.text);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}