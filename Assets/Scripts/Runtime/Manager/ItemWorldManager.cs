using Unity.Netcode;
using UnityEngine;
using System.Collections;
using static UnityEditor.Progress;
using Unity.VisualScripting;
using System;
using System.Collections.Generic;
using System.Linq;

public class ItemWorldManager : Singleton<ItemWorldManager>, IDataPersistence
{
    private ListItemWorld _listItemWorld;
    public NetworkList<ItemWorldNetworkData> networkItemWorldList;
    public GameObject itemDropPrefab;
    public ItemWorldControl[] itemsOnMap;

    public void OnEnable()
    {
        // Always construct your NetworkList before spawning
        if (networkItemWorldList == null)
        {
            networkItemWorldList = new NetworkList<ItemWorldNetworkData>(
                writePerm: NetworkVariableWritePermission.Server,
                readPerm: NetworkVariableReadPermission.Everyone);
        }
    }
    public override void OnNetworkSpawn() 
    {
        // this run everytime a player join and run before the load game
        if (networkItemWorldList.Count == 0) return; // host load game first time, network list is empty
        _listItemWorld = new ListItemWorld();
        foreach (var item in networkItemWorldList)
        {
            var itemWorld = NetworkVariableConverter.ItemWorldFromNetwork(item);
            _listItemWorld.AddItemWorld(itemWorld);
        }
        Debug.Log("item world in local list after client load: ");
        foreach (var item in _listItemWorld.Items)
        {
            Debug.Log(item.Id);
        }

        networkItemWorldList.OnListChanged += HandleNetworkListChanged;
    }

    

    public override void OnNetworkDespawn()
    {
        networkItemWorldList.OnListChanged -= HandleNetworkListChanged;
    }

    private void AddItemWorldIntoNetworkList(ListItemWorld itemWorlds)
    {
        foreach (var item in itemWorlds.Items)
        {
            Debug.Log(item.Id);
            networkItemWorldList.Add(NetworkVariableConverter.ItemWorldToNetwork(item));
        }

        foreach (var networkItem in networkItemWorldList)
        {
            Debug.Log(networkItem.Id);
        }

        networkItemWorldList.OnListChanged += HandleNetworkListChanged;
    }

    private void HandleNetworkListChanged(NetworkListEvent<ItemWorldNetworkData> changeEvent)
    {
        switch(changeEvent.Type)
        {
            case NetworkListEvent<ItemWorldNetworkData>.EventType.Add:
                var itemWorld = NetworkVariableConverter.ItemWorldFromNetwork(changeEvent.Value);
                _listItemWorld.AddItemWorld(itemWorld);
                Debug.Log("ItemWorldManager: Add item world to local list: " + itemWorld.Id);
                break;
            case NetworkListEvent<ItemWorldNetworkData>.EventType.Remove:
                var itemToDelete = NetworkVariableConverter.ItemWorldFromNetwork(changeEvent.Value);
                _listItemWorld.RemoveItemWorld(itemToDelete);
                Debug.Log("ItemWorldManager: Remove item world from local list: " + itemToDelete.Id);
                break;
        }
    }

    public void FindItemInListAndInitializeAfterClientJoin(ItemWorldControl itemWorldControl, string id)
    {
        if (IsServer) return;
        Debug.Log("Start finding item in list and initialize after client join: " + id);
        var itemWorld = _listItemWorld.Items.Find(x => x.Id == id);
        itemWorldControl.InitialItemWorld(itemWorld);
    }

    public void SpawnItem()
    {
        
        foreach (var item in _listItemWorld.Items)
        {
            GameObject itemGO = Instantiate(itemDropPrefab, item.Position, Quaternion.identity);

            var itemNetworkObject = itemGO.GetComponent<NetworkObject>();
            itemNetworkObject.Spawn();
            itemNetworkObject.GetComponent<ItemWorldControl>().CanPickup.Value = true; // set to true when spawn item in world
            InitializeItemWorldOnFirstSpawnClientRpc(itemNetworkObject, NetworkVariableConverter.ItemWorldToNetwork(item));
        }

    }


    [ClientRpc]
    private void InitializeItemWorldOnFirstSpawnClientRpc(NetworkObjectReference itemWorldRef, ItemWorldNetworkData itemWorldData)
    {
        if(itemWorldRef.TryGet(out NetworkObject obj))
        {
            var itemWorld = NetworkVariableConverter.ItemWorldFromNetwork(itemWorldData);
            var itemWorldControl = obj.GetComponent<ItemWorldControl>();
            itemWorldControl.InitialItemWorld(itemWorld);
        } 
    }
    public void AddItemWorld(ItemWorld item)
    {
        var itemWorldNetworkData = NetworkVariableConverter.ItemWorldToNetwork(item);
        if (networkItemWorldList.Contains(itemWorldNetworkData)) return;
        networkItemWorldList.Add(itemWorldNetworkData);
        Debug.Log("ItemWorldManager: Add item world to local list: " + itemWorldNetworkData.Id.ToString());
    }


    public void RemoveItemWorld(ItemWorld item, ItemWorldControl itemWorldControl)
    {
        RequestToRemoveItemWorldServerRpc(NetworkVariableConverter.ItemWorldToNetwork(item), itemWorldControl.GetComponent<NetworkObject>()); // send to server to remove item world

    } 

    [ServerRpc(RequireOwnership = false)]
    public void RequestToRemoveItemWorldServerRpc(ItemWorldNetworkData itemWorldNetworkData, NetworkObjectReference itemWorldRef)
    {
        Debug.Log("[Server] Received RPC to remove item");
        if (!networkItemWorldList.Contains(itemWorldNetworkData))
        {
            Debug.LogWarning("[Server] Item not found in list");
            return;
        }


        if (itemWorldRef.TryGet(out NetworkObject obj))
        {
            Debug.Log($"[Server] Despawning item: {obj.name}");

            networkItemWorldList.Remove(itemWorldNetworkData);
            obj.Despawn();
        }
        else
        {
            Debug.LogWarning("[Server] Could not resolve NetworkObjectReference");
        }

    }
    public void DropItemIntoWorld(ItemWorld itemWorldDropInfo, bool dropByStack, bool dropByPlayer = false)
    {
        var networkData = NetworkVariableConverter.ItemWorldToNetwork(itemWorldDropInfo); // put quantity and position to drop in here
        Debug.Log(networkData.Quantity);
        RequestToDropItemServerRpc(networkData,dropByStack, dropByPlayer);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestToDropItemServerRpc(ItemWorldNetworkData itemWorldNetworkData , bool dropByStack, bool dropByPlayer, ServerRpcParams rpcParams = default)
    {
        if (dropByPlayer)
        {
            ulong senderClientId = rpcParams.Receive.SenderClientId;
            var player = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject;
            itemWorldNetworkData.Position = player.transform.position;
        }

        if (dropByStack || itemWorldNetworkData.Quantity == 1)
            SpawnStackOfItem(itemWorldNetworkData);
        else
            SpawnEachOfItem(itemWorldNetworkData);

    }

    private void SpawnStackOfItem(ItemWorldNetworkData itemWorldNetworkData)
    {
        Vector3 itemPos = itemWorldNetworkData.Position;
        Vector3 randomDir = UtilsClass.GetRandomDir();
        Vector3 dropPos = itemPos + randomDir * 0.2f;

        itemWorldNetworkData.Position = dropPos;

        SpawnItemByServer(itemWorldNetworkData, randomDir);
    }

    private void SpawnEachOfItem(ItemWorldNetworkData itemWorldNetworkData)
    {
        
        for (int i = 0; i < itemWorldNetworkData.Quantity; i++)
        {
            Vector3 itemPos = itemWorldNetworkData.Position;
            Vector3 randomDir = UtilsClass.GetRandomDir();
            Vector3 dropPos = itemPos + randomDir * 0.2f;

            var singleData = itemWorldNetworkData;
            singleData.Position = dropPos;
            singleData.Quantity = 1;
            singleData.Id = System.Guid.NewGuid().ToString();

            SpawnItemByServer(singleData, randomDir);
        }
    }

    private void SpawnItemByServer(ItemWorldNetworkData itemWorldNetworkData, Vector3 randomDir)
    {
        GameObject newItemObject = Instantiate(itemDropPrefab, itemWorldNetworkData.Position, Quaternion.identity);

        var newItemNetworkObject = newItemObject.GetComponent<NetworkObject>();
        newItemNetworkObject.Spawn();

        var itemWorldControl = newItemNetworkObject.GetComponent<ItemWorldControl>();
        itemWorldControl.StartWaitForPickup(5f);

        AddItemWorld(NetworkVariableConverter.ItemWorldFromNetwork(itemWorldNetworkData));
        SetItemDropStatusClientRpc(itemWorldNetworkData, newItemNetworkObject, randomDir);
    }
    [ClientRpc]
    private void SetItemDropStatusClientRpc(ItemWorldNetworkData itemWorldNetworkData, NetworkObjectReference itemNetworkObject, Vector3 randomDir)
    {
        if (itemNetworkObject.TryGet(out NetworkObject obj))
        {
            var itemWorld = NetworkVariableConverter.ItemWorldFromNetwork(itemWorldNetworkData);
            var itemWorldControl = obj.GetComponent<ItemWorldControl>();
            itemWorldControl.GetComponent<Rigidbody2D>().AddForce(randomDir * 1f, ForceMode2D.Impulse);
            itemWorldControl.InitialItemWorld(itemWorld);

        }
    }
    public void LoadData(GameData gameData)
    {
        // only runs first time the host enter the game
        Debug.Log("Server load item world data");
        
        _listItemWorld = gameData.ListItemWold;
        itemsOnMap = FindObjectsOfType<ItemWorldControl>();

        if (_listItemWorld.Items == null || _listItemWorld.Items.Count == 0) // neu ko co item nao trong world dc luu trong file save truoc do
        {
            _listItemWorld = new ListItemWorld();
            foreach (var item in itemsOnMap) // khi add item vao scene truoc khi bat dau game thi se xai cai nay
            {
                ItemWorld itemWorld = item.GetItemWorld();
                _listItemWorld.AddItemWorld(itemWorld);
                Destroy(item.gameObject);
            }
        }
        else // neu co item trong file save thi se xai cai nay
        {
            ItemDatabase.Instance.SetItem(_listItemWorld.Items); // cap nhat lai item trong file save
            foreach (var item in itemsOnMap)
            {
                bool existItem = _listItemWorld.Items.Find(x => x.Id == item.id.Value) != null ? true : false;
                if (!existItem) // neu item truoc khi game bat dau nay ko ton tai trong file save thi add vao list
                {
                    ItemWorld itemWorld = item.GetItemWorld();
                    _listItemWorld.AddItemWorld(itemWorld);
                }
                Destroy(item.gameObject);
            }

        }
        if (_listItemWorld?.Items?.Count > 0)
        {
            SpawnItem();
        }

        AddItemWorldIntoNetworkList(_listItemWorld);
    }

    public void SaveData(ref GameData gameData)
    {
        Debug.Log("Start to save item world");
        foreach (var item in _listItemWorld.Items)
        {
            Debug.Log(item.Id);
        }
        gameData.SetListItemWorld(_listItemWorld);
    }

}
