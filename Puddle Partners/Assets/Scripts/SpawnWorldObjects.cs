using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SpawnWorldObjects : NetworkBehaviour
{

    [SerializeField]
    private GameObject rock;
    [SerializeField]
    private Vector3[] rockPositions;
    private GameObject[] rockObjs;
    [SerializeField]
    private GameObject umbrella;
    [SerializeField]
    private Vector3[] umbrellaPositions;
    private GameObject[] umbrellaObjs;
    private bool scLoad = false;

    void Start()
    {
        if (NetworkManager.IsHost)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += Sceneloaded;
        }
        rockObjs = new GameObject[rockPositions.Length];
        umbrellaObjs = new GameObject[umbrellaPositions.Length];
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= Sceneloaded;
        }
    }

    private void Update()
    {
        if (scLoad)
        {
            for (int i = 0; i < rockPositions.Length; i++)
            {
                if (rockObjs[i].transform.localPosition.y < -100.0f)
                {
                    rockObjs[i].transform.localPosition = rockPositions[i];
                }
            }
            for (int i = 0; i < umbrellaPositions.Length; i++)
            {
                if (umbrellaObjs[i].transform.localPosition.y < -100.0f)
                {
                    umbrellaObjs[i].transform.localPosition = umbrellaPositions[i];
                }
            }
        }
    }

    private void Sceneloaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        for (int i = 0; i < rockPositions.Length; i++)
        {
            GameObject tmpObj = Instantiate(rock, rockPositions[i], new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            tmpObj.GetComponent<NetworkObject>().Spawn(tmpObj);
            rockObjs[i] = tmpObj;
            Debug.Log("Spawn Rock");
        }
        for (int i = 0; i < umbrellaPositions.Length; i++)
        {
            GameObject tmpObj = Instantiate(umbrella, umbrellaPositions[i], new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            tmpObj.GetComponent<NetworkObject>().Spawn(tmpObj);
            umbrellaObjs[i] = tmpObj;
            Debug.Log("Spawn Umbrella");
        }
        scLoad = true;
    }

}