using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using Unity.Collections;

public class Lobby : NetworkBehaviour
{

    public TMP_Text lobbyId;
    public TMP_Text lobbyHost;
    public TMP_Text lobbyClient;
    public GameObject lobbyIdText;
    public GameObject destroySession;
    public GameObject lobby;
    public GameObject mainMenu;
    public GameObject selectedLevelButton;
    private NetworkVariable<int> hostId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<FixedString128Bytes> hostName = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> clientId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<FixedString128Bytes> clientName = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.IsHost)
        {
            lobbyId.text = "Lobby Session ID: " + PlayerPrefs.GetString("JoinCode");
            lobbyHost.text = "Spieler 1: " + PlayerPrefs.GetString("LoginName");
            HostServerRpc(Int32.Parse(PlayerPrefs.GetString("LoginID")), PlayerPrefs.GetString("LoginName"));
            lobbyIdText.SetActive(true);
        } else if(!NetworkManager.IsHost && NetworkManager.IsClient)
        {
            lobbyClient.text = "Spieler 2: " + PlayerPrefs.GetString("LoginName");
            ClientServerRpc(Int32.Parse(PlayerPrefs.GetString("LoginID")), PlayerPrefs.GetString("LoginName"));

        }
    }

    public void OnClientConnectedCallback(ulong id)
    {
        if (NetworkManager.IsHost)
        {
            if (id != 0)
            {
                lobbyClient.text = "Spieler 2: " + clientName.Value.ToString();
                PlayerPrefs.SetString("HostID", hostId.Value.ToString());
                PlayerPrefs.SetString("HostName", hostName.Value.ToString());
                PlayerPrefs.SetString("ClientID", clientId.Value.ToString());
                PlayerPrefs.SetString("ClientName", clientName.Value.ToString());
                selectedLevelButton.SetActive(true);
            }
        } else
        {
            lobbyHost.text = "Spieler 1: " + hostName.Value.ToString();
            PlayerPrefs.SetString("HostID", hostId.Value.ToString());
            PlayerPrefs.SetString("HostName", hostName.Value.ToString());
            PlayerPrefs.SetString("ClientID", clientId.Value.ToString());
            PlayerPrefs.SetString("ClientName", clientName.Value.ToString());
        }
    }

    public void OnClientDisconnectCallback(ulong id)
    {
        if (NetworkManager.IsHost)
        {
            lobbyClient.text = "Spieler 2:";
            PlayerPrefs.SetString("HostID", "");
            PlayerPrefs.SetString("HostName", "");
            PlayerPrefs.SetString("ClientID", "");
            PlayerPrefs.SetString("ClientName", "");
            ClientServerRpc(0, "");
            selectedLevelButton.SetActive(false);
        }
        else
        {
            lobby.SetActive(false);
            mainMenu.SetActive(true);
            PlayerPrefs.SetString("HostID", "");
            PlayerPrefs.SetString("HostName", "");
            PlayerPrefs.SetString("ClientID", "");
            PlayerPrefs.SetString("ClientName", "");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void HostServerRpc(int id, FixedString128Bytes name, ServerRpcParams serverRpcParams = default)
    {
        hostId.Value = id;
        hostName.Value = name;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientServerRpc(int id, FixedString128Bytes name, ServerRpcParams serverRpcParams = default)
    {
        clientId.Value = id;
        clientName.Value = name;
    }

    public void Disconnect()
    {
        lobby.SetActive(false);
        mainMenu.SetActive(true);
        NetworkManager.Singleton.Shutdown();
    }

}