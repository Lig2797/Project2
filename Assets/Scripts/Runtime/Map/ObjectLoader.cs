using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ObjectLoader : MonoBehaviour
{
    public Camera cam;
    public int buffer = 5;

    public List<string> tagFilters = new List<string>();

    [HideInInspector]
    public List<GameObject> allObjects = new();
    private Dictionary<Vector3Int, List<GameObject>> objectsByCell = new();

    private Vector3Int lastMin, lastMax;
    private int cellSize = 1;

    public void ScanAndBuildGrid()
    {
        objectsByCell.Clear();

        foreach (var obj in allObjects)
        {
            Vector3Int cell = WorldToCell(obj.transform.position);
            if (!objectsByCell.ContainsKey(cell))
                objectsByCell[cell] = new List<GameObject>();

            objectsByCell[cell].Add(obj);
            obj.SetActive(false);
        }
    }

    void Start()
    {
        ScanAndBuildGrid();
    }

    void Update()
    {
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1));

        Vector3Int minCell = WorldToCell(min) - Vector3Int.one * buffer;
        Vector3Int maxCell = WorldToCell(max) + Vector3Int.one * buffer;

        if (minCell != lastMin || maxCell != lastMax)
        {
            HashSet<GameObject> visible = new();

            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector3Int cell = new(x, y);
                    if (objectsByCell.TryGetValue(cell, out var objList))
                    {
                        foreach (var obj in objList)
                        {
                            obj.SetActive(true);
                            visible.Add(obj);
                        }
                    }
                }
            }

            foreach (var kv in objectsByCell)
            {
                foreach (var obj in kv.Value)
                {
                    if (!visible.Contains(obj))
                        obj.SetActive(false);
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
