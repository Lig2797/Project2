using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkConnectManager : Singleton<NetworkConnectManager>
{
    private Dictionary<ulong, PlayerController> connectedPlayers;

    private void Start()
    {
        StartCoroutine(WaitAndSubscribe());
    }

    private IEnumerator WaitAndSubscribe()
    {
        if (!NetworkManager.Singleton.IsListening && NetworkManager.Singleton == null)
            yield return new WaitUntil(() => NetworkManager.Singleton.IsListening);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    // Runs only on the server
    void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            Debug.LogError($"Client {clientId} not found in ConnectedClients.");
            return;
        }

        var networkObject = client.PlayerObject;
        var playerController = networkObject.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController is null");
            return;
        }

        bool isHost = clientId == NetworkManager.ServerClientId;

        if (isHost)
        {
            connectedPlayers = new Dictionary<ulong, PlayerController>();
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        connectedPlayers[clientId] = playerController;
    }


    void OnClientDisconnected(ulong clientId)
    {   
        Debug.Log($"Client {clientId} disconnected.");
        if (!NetworkManager.Singleton.IsServer)
            return;
        if (connectedPlayers.TryGetValue(clientId, out var playerController))
        {
            connectedPlayers.Remove(clientId);
        }
        else
        {
            Debug.LogWarning($"PlayerController not found for clientId {clientId}");
        }
    }
}
