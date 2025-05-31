using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CaveManager : Singleton<CaveManager>
{
    public NetworkDictionary<int, int> caveList;
    public int CurrentLocalCaveLevel = 1;

    public enum SpecialTileType { None, DownStair, UpStair, Exit }
    public Dictionary<Vector3Int, SpecialTileType> localStairTiles = new Dictionary<Vector3Int, SpecialTileType>();
    private void OnEnable()
    {
        if(caveList == null)
        caveList = new NetworkDictionary<int, int>();
    }



    public int GetCaveLevelFromNetwork(int caveLevel)
    {
        if (caveList == null || caveList.Count == 0)
        {
            Debug.Log("Cave list is empty or not initialized.");
            return -1; // Return -1 if the cave list is empty or not initialized
        }
        foreach (var cave in caveList)
        {
            if (cave.Key == caveLevel)
            {
                return cave.Value;
            }
        }
        return -1; // Return -1 if the cave number is not found
    }

    public void AddCaveLevel(int caveLevel, int caveNumber)
    {
        if (!caveList.ContainsKey(caveLevel))
        caveList[caveLevel] = caveNumber;
    }

    public void RefreshCurrentStairTile()
    {
        localStairTiles.Clear();
    }

    public void AddStairTile(Vector3Int pos, SpecialTileType type)
    {
        localStairTiles[pos] = type;
    }
}
