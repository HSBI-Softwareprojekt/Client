using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class Relay : NetworkBehaviour
{

    private NetworkVariable<int> hostId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> clientId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        { };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public override void OnNetworkSpawn()
    {
        hostId.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log("Host ID: " + hostId.Value);
        };
        clientId.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log("Client ID: " + clientId.Value);
        };
    }

    public void Update()
    {
        if(IsHost && hostId.Value == 0)
        {
            hostId.Value = Int32.Parse(PlayerPrefs.GetString("LoginID"));
        }
        if(!IsHost && IsClient && clientId.Value == 0)
        {
            clientId.Value = Int32.Parse(PlayerPrefs.GetString("LoginID"));
        }
        if(hostId.Value != 0)
        {
            Debug.Log("Host ID: " + hostId.Value);
        }
        if (clientId.Value != 0)
        {
            Debug.Log("Client ID: " + clientId.Value);
        }
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            //hostId.Value = Int32.Parse(PlayerPrefs.GetString("LoginID"));
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }

        public async void JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            clientId.Value = Int32.Parse(PlayerPrefs.GetString("LoginID"));
        }
        catch (RelayServiceException ex)
        {
           Debug.Log(ex);
        }
    }

}