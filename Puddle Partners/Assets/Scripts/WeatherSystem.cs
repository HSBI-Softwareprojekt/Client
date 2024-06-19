using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    public GameObject RainGroundPrefab;
    public GameObject RainAcidPrefab;

    private GameObject rainGroundInstance;
    private GameObject rainAcidInstance;
    private string previousWeather;

    void Start()
    {
        rainGroundInstance = Instantiate(RainGroundPrefab);
        rainGroundInstance.SetActive(false);

        rainAcidInstance = Instantiate(RainAcidPrefab);
        rainAcidInstance.SetActive(false);

        previousWeather = "RainGround";

        ChangeWeather();
    }

    void ChangeWeather()
    {
        rainGroundInstance.SetActive(false);
        rainAcidInstance.SetActive(false);

        float randomValue = Random.Range(0f, 1f);
        string newWeather;

        if (previousWeather == "Clear")
        {
            if (randomValue < 0.4f)
            {
                newWeather = "Clear";
            }
            else if (randomValue < 0.8f)
            {
                newWeather = "RainGround";
            }
            else
            {
                newWeather = "RainAcid";
            }
        }
        else
        {
            if (randomValue < 0.6f)
            {
                newWeather = "Clear";
            }
            else if (randomValue < 0.9f)
            {
                newWeather = "RainGround";
            }
            else
            {
                newWeather = "RainAcid";
            }
        }

        switch (newWeather)
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

        previousWeather = newWeather;

        Invoke("ChangeWeather", Random.Range(5f, 10f));
    }
}