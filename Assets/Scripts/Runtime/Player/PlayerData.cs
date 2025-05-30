using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerDataNetwork : IEquatable<PlayerDataNetwork>, INetworkSerializable
{
    public ulong clientId;
    public int characterId;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;

    public bool Equals(PlayerDataNetwork other)
    {
        return
            clientId == other.clientId &&
            characterId == other.characterId &&
            playerName == other.playerName &&
            playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref characterId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }
}

[System.Serializable]
public class PlayerData
{
    [SerializeField] private PlayerDataNetwork _playerDataNetwork;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxMana;
    [SerializeField] private float _currentMana;
    [SerializeField] private float _maxStamina;
    [SerializeField] private float _currentStamina;
    [SerializeField] private float _money;
    [SerializeField] private Vector3 _position;

    public float MaxHealth
    { get { return _maxHealth; } } 

    public float CurrentHealth 
    { get { return _currentHealth; } }

    public float MaxMana
    { get { return _maxMana; } }

    public float CurrentMana
    { get { return _currentMana; } }

    public float MaxStamina
    { get { return _maxStamina; } }

    public float CurrentStamina
    { get { return _currentStamina; } }

    public float Money
    { get { return _money; } }

    public Vector3 Position
    { get { return _position; } }

    public PlayerData() // Create default Player state and stats when open game
    {
        this._maxHealth = 100;
        this._currentHealth = 100;
        this._maxMana = 100;
        this._currentMana = 100;
        this._maxStamina = 100;
        this._currentStamina = 100;
        this._money = 0;
        this._position = new Vector3(4.371f, 1.154f, 0);
    }

    // Health
    public void SetMaxHealth(float health)
    {
        this._maxHealth = health;
    }
    // Current health
    public void SetCurrentHealth(float health)
    {
        this._currentHealth = health;
    }

    // Money
    public void SetMoney(float money)
    {
        this._money = money;
    }

    // Position
    public void SetPosition(Vector3 position)
    {
        this._position = position;
    }
}
