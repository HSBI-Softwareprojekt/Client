using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using UnityEngine.SceneManagement;
using Unity.Collections;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class WeatherSystem : NetworkBehaviour
{
    public GameObject RainGroundPrefab;
    public GameObject RainAcidPrefab;

    private NetworkVariable<FixedString128Bytes> weatherType = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private GameObject rainGroundInstance;
    private GameObject rainAcidInstance;
    private string previousWeather;

    void Start()
    {
        rainGroundInstance = Instantiate(RainGroundPrefab);
        rainGroundInstance.SetActive(false);

        rainAcidInstance = Instantiate(RainAcidPrefab);
        rainAcidInstance.SetActive(false);

        previousWeather = "";

        weatherType.OnValueChanged += ChangeWeather;

        if(NetworkManager.IsHost)
        {
            Weather();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void WeatherChangeServerRpc(FixedString128Bytes weatherTypeRpc, ServerRpcParams serverRpcParams = default)
    {
        weatherType.Value = weatherTypeRpc;
    }

    void Weather()
    {
        rainGroundInstance.SetActive(false);
        rainAcidInstance.SetActive(false);

        float randomValue = Random.Range(0f, 1f);
        string newValue = "";

        if(previousWeather == "")
        {
            previousWeather = "Clear";
            newValue = "Clear";
        }
        else{
            if (previousWeather == "Clear")
            {
                if (randomValue < 0.4f)
                {
                    newValue = "Clear";
                }
                else if (randomValue < 0.8f)
                {
                    newValue = "RainGround";
                }
                else
                {
                    newValue = "RainAcid";
                }
            }
            else
            {
                if (randomValue < 0.6f)
                {
                    newValue = "Clear";
                }
                else if (randomValue < 0.9f)
                {
                    newValue = "RainGround";
                }
                else
                {
                    newValue = "RainAcid";
                }
            }
        }

        switch (newValue)
        {
            case "Clear":
                Debug.Log("Clear Weather");
                break;
            case "RainGround":
                Debug.Log("Normal Rain");
                rainGroundInstance.SetActive(true);
                break;
            case "RainAcid":
                Debug.Log("Acid Rain");
                rainAcidInstance.SetActive(true);
                break;
        }

        previousWeather = newValue;
        WeatherChangeServerRpc(newValue);
        Invoke("Weather", Random.Range(5f, 10f));
    }


    void ChangeWeather(FixedString128Bytes oldValue, FixedString128Bytes newValue)
    {
        if(!NetworkManager.IsHost){
            rainGroundInstance.SetActive(false);
            rainAcidInstance.SetActive(false);
            switch (newValue.ToString())
        {
            case "Clear":
                Debug.Log("Clear Weather");
                break;
            case "RainGround":
                Debug.Log("Normal Rain");
                rainGroundInstance.SetActive(true);
                break;
            case "RainAcid":
                Debug.Log("Acid Rain");
                rainAcidInstance.SetActive(true);
                break;
        }
        }
    }
}