using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

// Script for building a Connection
public class Relay : NetworkBehaviour
{
    // Handle Authentication
    public async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => { };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Create a Relay Connection as a Host
    public async void CreateRelay()
    {
        try
        {
            // The Connection, that CLients can Connect to with a Max Amount of Connections being 1
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            // The Code that allows the Clients to join the host
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            // Set the Join Code, so that it can be accessed and displayed
            PlayerPrefs.SetString("JoinCode", joinCode);
            // Create the Relay Server Data based on the allocation
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            // Set the Data of our Connection
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            // Start the Host with the established Connection
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    // Join a Relay Connection with a Code as a Client
    public async void JoinRelay(string joinCode)
    {
        try
        {
            // Build a Connection for joining
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            // Create the Relay Server Data based on the allocation
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            // Set the Data of our Connection
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            // Start the Client with the established Connection
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException ex)
        {
           Debug.Log(ex);
        }
    }

}