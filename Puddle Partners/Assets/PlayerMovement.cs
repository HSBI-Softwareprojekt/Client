using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController2D controller;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private AudioListener listener;
    public float runSpeed = 20f;
    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if(Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            listener.enabled = true;
            virtualCamera.Priority = 1;
        }
        else
        {
            virtualCamera.Priority = 0;
        }
    }
}
