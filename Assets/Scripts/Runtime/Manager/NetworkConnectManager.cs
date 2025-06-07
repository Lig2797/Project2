using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkConnectManager : NetworkPersistentSingleton<NetworkConnectManager>
{
    private NetworkList<PlayerDataNetwork> connectedPlayers = new();

    //private void OnEnable()
    //{
    //    if (connectedPlayers != null)
    //        connectedPlayers = new();
    //}
    private void Start()
    {
        StartCoroutine(WaitAndSubscribe());
    }

    private IEnumerator WaitAndSubscribe()
    {
        if (NetworkManager.Singleton == null)
            yield return new WaitUntil(() => NetworkManager.Singleton != null);

        yield return new WaitUntil(() => NetworkManager.Singleton.IsListening);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    // Runs only on the server
    void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

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
            connectedPlayers = new NetworkList<PlayerDataNetwork>();
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        connectedPlayers.Add(playerController.playerDataNetwork);

        SetPlayerNameServerRpc(playerController.playerDataSO.playerName.ToString());
        SetCharactersAnimatorServerRpc(playerController.playerDataSO.characterId);
    }


    void OnClientDisconnected(ulong clientId)
    {   
        Debug.Log($"Client {clientId} disconnected.");
        if (!NetworkManager.Singleton.IsServer)
            return;
        if (connectedPlayers[(int)clientId].clientId == clientId)
        {
            connectedPlayers.RemoveAt((int)clientId);
        }
        else
        {
            Debug.LogWarning($"PlayerController not found for clientId {clientId}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerDataNetwork playerData = connectedPlayers[playerDataIndex];

        playerData.playerName = playerName;

        connectedPlayers[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetCharactersAnimatorServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"charID: {characterId}");
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerDataNetwork playerData = connectedPlayers[playerDataIndex];

        playerData.characterId = characterId;

        connectedPlayers[playerDataIndex] = playerData;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        return (int)clientId;
    }
}
