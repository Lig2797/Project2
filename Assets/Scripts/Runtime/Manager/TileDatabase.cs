using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDatabase : PersistentSingleton<TileDatabase>
{
    [SerializeField]
    private List<TileBase> tileBases;

    public TileBase GetTileByName(string tileName)
    {
        foreach(TileBase tile in tileBases)
        {
            if (tile.name == tileName)
            {
                return tile;
            }
        }
        Debug.LogWarning($"Tile with name {tileName} not found in the database.");
        return null;
    }
}
