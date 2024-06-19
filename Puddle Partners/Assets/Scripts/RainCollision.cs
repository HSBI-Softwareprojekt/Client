using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainCollision : MonoBehaviour
{
    public ParticleSystem RainGround;
    void Start()
    {
        var collision = RainGround.collision;
        if(!collision.enabled)
        {
            collision.enabled = true;
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player hit by rain!");
        }
    }
}
