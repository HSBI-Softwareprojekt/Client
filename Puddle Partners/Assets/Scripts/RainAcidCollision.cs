using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainAcidCollision : MonoBehaviour
{
    public ParticleSystem RainAcid;
    void Start()
    {
        var collision = RainAcid.collision;
        if(!collision.enabled)
        {
            collision.enabled = true;
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player hit by acid rain!");
        }
    }
}
