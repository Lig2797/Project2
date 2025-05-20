using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public static class NetworkVariableConverter
{
    public static ItemWorldNetworkData ItemWorldToNetwork(ItemWorld itemWorld)
    {
        return new ItemWorldNetworkData
        {
            Id = itemWorld.Id,
            ItemName = itemWorld.ItemName,
            Quantity = itemWorld.Quantity,
            Position = itemWorld.Position
        };
    }

    public static ItemWorld ItemWorldFromNetwork(ItemWorldNetworkData data)
    {
        Item item = ItemDatabase.Instance.GetItemByName(data.ItemName.ToString()); // lookup from string
        return new ItemWorld(
            data.Id.ToString(),
            item,
            data.Quantity,
            data.Position
        );
    }

    public static List<ItemWorld> ItemWorldListFromNetwork(List<ItemWorldNetworkData> data)
    {
        List<ItemWorld> itemWorlds = new List<ItemWorld>();
        foreach (var item in data)
        {
            itemWorlds.Add(ItemWorldFromNetwork(item));
        }
        return itemWorlds;
    }
}
[Serializable]
public struct ItemWorldNetworkData : INetworkSerializable, IEquatable<ItemWorldNetworkData>
{
    public FixedString64Bytes Id;
    public FixedString64Bytes ItemName;
    public int Quantity;
    public Vector3 Position;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref ItemName);
        serializer.SerializeValue(ref Quantity);
        serializer.SerializeValue(ref Position);
    }

    public bool Equals(ItemWorldNetworkData other)
    {
        return Id.Equals(other.Id)
            && ItemName.Equals(other.ItemName)
            && Quantity == other.Quantity
            && Position.Equals(other.Position);
    }

    public override bool Equals(object obj)
    {
        return obj is ItemWorldNetworkData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ItemName, Quantity, Position);
    }
}

