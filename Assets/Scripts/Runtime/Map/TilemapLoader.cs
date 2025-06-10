using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class TilemapLoader : MonoBehaviour
{
    [System.Serializable]
    public class LayerLoader
    {
        public string layerName;
        public Tilemap tilemap;
    }

    public MapGroupData mapGroupData;
    public Camera cam;
    public int buffer = 5;
    public LayerLoader[] layerLoaders;

    private Dictionary<string, Dictionary<Vector3Int, TileBase>> tileDataPerLayer = new();
    private Dictionary<string, HashSet<Vector3Int>> activeTilesPerLayer = new();
    private Vector3Int lastMin, lastMax;
    private Coroutine loadingRoutine;

    void Start()
    {
        foreach (var layer in mapGroupData.layers)
        {
            var dict = new Dictionary<Vector3Int, TileBase>();
            foreach (var tile in layer.tiles)
                dict[tile.position] = tile.tile;

            tileDataPerLayer[layer.layerName] = dict;
            activeTilesPerLayer[layer.layerName] = new HashSet<Vector3Int>();
        }
    }

    void Update()
    {
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1));

        Vector3Int minCell = layerLoaders[0].tilemap.WorldToCell(min) - new Vector3Int(buffer, buffer, 0);
        Vector3Int maxCell = layerLoaders[0].tilemap.WorldToCell(max) + new Vector3Int(buffer, buffer, 0);

        if (minCell != lastMin || maxCell != lastMax)
        {
            if (loadingRoutine != null) StopCoroutine(loadingRoutine);
            loadingRoutine = StartCoroutine(LoadVisibleTiles(minCell, maxCell));
            lastMin = minCell;
            lastMax = maxCell;
        }
    }

    IEnumerator LoadVisibleTiles(Vector3Int minCell, Vector3Int maxCell)
    {
        foreach (var layer in layerLoaders)
        {
            var tileDict = tileDataPerLayer[layer.layerName];
            var activeSet = activeTilesPerLayer[layer.layerName];
            var newActive = new HashSet<Vector3Int>();

            int processed = 0;

            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if (tileDict.TryGetValue(pos, out var tile))
                    {
                        if (!activeSet.Contains(pos))
                            layer.tilemap.SetTile(pos, tile);

                        newActive.Add(pos);

                        if (++processed >= 100)
                        {
                            processed = 0;
                            yield return null;
                        }
                    }
                }
            }

            foreach (var pos in activeSet)
            {
                if (!newActive.Contains(pos))
                {
                    layer.tilemap.SetTile(pos, null);
                    if (++processed >= 100)
                    {
                        processed = 0;
                        yield return null;
                    }
                }
            }

            activeTilesPerLayer[layer.layerName] = newActive;
        }
    }
}