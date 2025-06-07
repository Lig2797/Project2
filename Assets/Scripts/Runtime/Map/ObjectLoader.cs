using UnityEngine;
using System.Collections.Generic;

public class ObjectLoader : MonoBehaviour
{
    public Camera cam;
    public int buffer = 5;
    public TaggedObjectList objectData;

    private Dictionary<Vector3Int, List<TaggedObjectList.TaggedObjectData>> cellToData = new();
    private Dictionary<Vector3, GameObject> spawnedObjects = new();
    private Vector3Int lastMin, lastMax;
    private int cellSize = 1;

    void Start()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        foreach (var data in objectData.allObjects)
        {
            Vector3Int cell = WorldToCell(data.position);
            if (!cellToData.ContainsKey(cell))
                cellToData[cell] = new List<TaggedObjectList.TaggedObjectData>();

            cellToData[cell].Add(data);
        }
    }

    void Update()
    {
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1));

        Vector3Int minCell = WorldToCell(min) - Vector3Int.one * buffer;
        Vector3Int maxCell = WorldToCell(max) + Vector3Int.one * buffer;

        if (minCell != lastMin || maxCell != lastMax)
        {
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector3Int cell = new(x, y);
                    if (cellToData.TryGetValue(cell, out var list))
                    {
                        foreach (var objData in list)
                        {
                            if (spawnedObjects.ContainsKey(objData.position)) continue;

                            if (objData.prefab != null)
                            {
                                GameObject obj = Instantiate(objData.prefab, objData.position, Quaternion.identity);
                                obj.tag = objData.tag;
                                spawnedObjects[objData.position] = obj;
                            }
                            else
                            {
                                Debug.LogWarning($"Missing prefab for object at {objData.position}");
                            }
                        }
                    }
                }
            }

            lastMin = minCell;
            lastMax = maxCell;
        }
    }

    Vector3Int WorldToCell(Vector3 pos)
    {
        return new Vector3Int(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.y / cellSize));
    }
}
