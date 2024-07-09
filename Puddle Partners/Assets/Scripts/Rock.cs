using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Rock : NetworkBehaviour
{/*

    private bool grab = false;
    [SerializeField]
    private GameObject rock;
    private ulong playerId;
    private GameObject[] player;
    private bool inRange = false;

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("is in range");
            inRange = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("not in range");
            inRange = false;
        }

    }

    void Update()
    {
        Debug.Log(inRange);
        if(!grab && inRange)
        {
            if(Input.GetButton("Grab"))
            {
                if(!NetworkManager.IsHost)
                {
                    grab = true;
                    playerId = 1;
                } else
                {
                    grab = true;
                    playerId = 0;
                }
            }
        } else
        {
            if(playerId == 0 && NetworkManager.IsHost)
            {
                MoveRock(playerId);
            }
            else if(playerId == 1 && !NetworkManager.IsHost)
            {
                MoveRock(playerId);
            }
        }
    }

    private void MoveRock(ulong id)
    {
        if(!Input.GetButton("Grab"))
        {
            grab = false;
        } else
        {
            Transform objectHolder = player[id].transform.GetChild(5).gameObject.transform;
            NetworkObject obj = rock.GetComponent<NetworkObject>();
            obj.transform.SetParent(this.transform);
            obj.transform.position = objectHolder.position;
            obj.GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }
    */
}