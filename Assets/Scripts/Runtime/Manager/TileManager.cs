using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : Singleton<TileManager>, IDataPersistence
{
    [SerializeField] private List<Tilemap> tilemaps = new List<Tilemap>();
    [SerializeField] private List<RuleTile> ruleTiles = new List<RuleTile>();

    private string FarmGroundTilemapName = "FarmGround";
    private string WateredGroundTilemapName = "WateredGround";
    private string HoedTileName = "Hoed Tile";
    private string WateredTileName = "Watered Tile";

    private SerializableDictionary<Vector3Int, HoedTileData> _hoedTiles = new SerializableDictionary<Vector3Int, HoedTileData>();
    public SerializableDictionary<Vector3Int, HoedTileData> HoedTiles
    {
        get { return _hoedTiles; }
        set { _hoedTiles = value; }
    }
   
    private SerializableDictionary<Vector3Int, WateredTileData> _wateredTiles = new SerializableDictionary<Vector3Int, WateredTileData>();
    public SerializableDictionary<Vector3Int, WateredTileData> WateredTiles
    {
        get { return _wateredTiles; }
        set { _wateredTiles = value; }
    }

    private TileSaveData _tileSaveData = new TileSaveData();


    private void OnEnable()
    {
        EnviromentalStatusManager.OnTimeIncrease += UpdateAllTileStatus;
    }

    private void OnDisable()
    {
        EnviromentalStatusManager.OnTimeIncrease -= UpdateAllTileStatus;
    }

    private Tilemap GetTilemapByName(string tilemapName)
    {
        return tilemaps.Find(x => x.name == tilemapName);
    }
    private RuleTile GetRuleTileByName(string ruleTileName)
    {
        
        return ruleTiles.Find(x => x.name == ruleTileName);
    }
    public void UpdateAllTileStatus(int minute)
    {
        foreach (var hoedTile in HoedTiles.ToList())
        {
            Vector3Int hoedPosition = hoedTile.Key;
            HoedTileData hoedTileData = hoedTile.Value;

            if (WateredTiles.ContainsKey(hoedPosition) || CropManager.Instance.PlantedCrops.ContainsKey(hoedPosition))
            {
                hoedTileData.hasSomethingOn = true;
            }
            hoedTileData.CheckTile(minute);
            if (hoedTileData.needRemove)
            {
                ModifyTile(hoedPosition, FarmGroundTilemapName);
            }
             
        }

        foreach (var wateredTile in WateredTiles.ToList())
        {
            Vector3Int wateredPosition = wateredTile.Key;
            WateredTileData wateredTileData = wateredTile.Value;


            wateredTileData.CheckTile(minute);
            if (wateredTileData.needRemove)
            {
                ModifyTile(wateredPosition, WateredGroundTilemapName);
            }

        }
    }

    public void ModifyTile(Vector3Int tilePos, string tilemapName, string ruleTileName = null)
    {
     
        RequestToAddTileServerRpc(tilePos, tilemapName, ruleTileName);

        //HoedTileData newHoedTile = new HoedTileData();
        //_hoedTiles.Add(tilePos, newHoedTile);

    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestToAddTileServerRpc(Vector3Int tilePos, string tilemapName, string ruleTileName = null)
    {
        ApplyTileForPlayersClientRpc(tilePos, tilemapName, ruleTileName);
    }

    [ClientRpc]
    private void ApplyTileForPlayersClientRpc(Vector3Int tilePos, string tilemapName, string ruleTileName = null)
    {
        var targetTilemap = GetTilemapByName(tilemapName);
        var ruleTile = GetRuleTileByName(ruleTileName);

        if (targetTilemap != null)
        {
            targetTilemap.SetTile(tilePos, ruleTile);

            if (ruleTile != null)
            {
                switch (tilemapName) 
                {
                    default:
                        {
                            Debug.Log("this tilemap cant do anything" + tilemapName);
                            break;
                        }
                    case "FarmGround":
                        if(HoedTiles.ContainsKey(tilePos)) // check if tile already exist
                        {
                            Debug.Log("this position already contains the tile" + tilemapName);
                            break;
                        }
                        var newHoedTile = new HoedTileData();
                        _hoedTiles.Add(tilePos, newHoedTile);
                        break;

                    case "WateredGround":
                        if(WateredTiles.ContainsKey(tilePos)) // check if tile already exist
                        {
                            Debug.Log("this position already contains the tile" + tilemapName);
                            break;
                        }
                        var newWateredTile = new WateredTileData();
                        _wateredTiles.Add(tilePos, newWateredTile);
                        break;
                }

                
                
            }
            else // ruleTile = null means delete it
            {
                switch (tilemapName)
                {
                    default:
                        {
                            Debug.Log("this tilemap cant do anything" + tilemapName);
                            break;
                        }
                    case "FarmGround":
                        if(!HoedTiles.ContainsKey(tilePos)) // check if tile already exist
                        {
                            Debug.Log("this position not contains the tile" + tilemapName);
                            break;
                        }
                        _hoedTiles.Remove(tilePos);
                        if (WateredTiles.ContainsKey(tilePos)) ModifyTile(tilePos, tilemapName);
                        break;

                    case "WateredGround":
                        if (!WateredTiles.ContainsKey(tilePos)) // check if tile already exist
                        {
                            Debug.Log("this position not contains the tile" + tilemapName);
                            break;
                        }
                        _wateredTiles.Remove(tilePos);
                        break;
                }
            }
        }
        else
        {
            Debug.LogError("Tilemap not found");
        }
    }

    public void LoadData(GameData data)
    {
        HoedTiles = data.TileSaveData.HoedTiles;
        WateredTiles = data.TileSaveData.WateredTiles;

        StartCoroutine(ApplyTileUpdates(data));
    }
    private IEnumerator ApplyTileUpdates(GameData data)
    {
        yield return new WaitForEndOfFrame(); // Wait until rendering is done

        foreach (var hoedTile in HoedTiles)
        {
            ModifyTile(hoedTile.Key, FarmGroundTilemapName, HoedTileName);
        }

        foreach (var wateredTile in WateredTiles)
        {
            ModifyTile(wateredTile.Key, WateredGroundTilemapName, WateredTileName);
        }

        CropManager.Instance.LoadCrops(data.TileSaveData.CropTiles);
    }
    public void SaveData(ref GameData data)
    {
        _tileSaveData.SetTiles(HoedTiles, WateredTiles, CropManager.Instance.PlantedCrops);
        data.SetTiles(_tileSaveData);
    }

}
