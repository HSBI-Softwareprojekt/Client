using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Audio : NetworkBehaviour
{
    public GameObject audioObj;
    [SerializeField]
    private AudioSource[] audioSrc;
    private int start = -1;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Update()
    {
        bool play = false;
        for (int i = 0; i < audioSrc.Length; i++) {
            if (audioSrc[i].isPlaying)
            {
                play = true;
            }
        }
        if(!play)
        {
            if((start+1) == audioSrc.Length)
            {
                start = 0;
            } else
            {
                start++;
            }
            audioSrc[start].Play();
        }
    }

}
