using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif

public class TileTargeter : NetworkBehaviour
{
    #region Setup variables
    [SerializeField] PlayerController playerController;
    [SerializeField]
    private List<Tilemap> _tilemaps;
    
    [SerializeField]
    private Tilemap _targetTilemap;

    [Header("TARGET TILE SETTINGS")]
    [SerializeField]
    private AnimatedTile _targetTile;

    [SerializeField]
    private int TargetRange = 1;

    private Vector3 _mouseWorldPosition;
    private Vector3Int _previousTilePos;
    private Vector3Int _mouseTilePosition;
    private Vector3Int _playerTilePosition;
    private Vector3Int _clampedTilePosition;
    private Vector3Int _lockedTilePosition;

    [SerializeField] private List<Tilemap> tilemapCheck = new();
    [Header("HOE ON TILES SETTINGS")]
    [SerializeField] private bool _canHoe = false;
    public bool CanHoe
    {
        get { return _canHoe; }
        set { _canHoe = value; }
    }

    [SerializeField] private bool _lockedCanHoe = false;
    public bool LockedCanHoe
    {
        get { return _lockedCanHoe; }
        set { _lockedCanHoe = value; }
    }


    [Header("WATER ON TILES SETTINGS")]
    [SerializeField] private bool _canWater = false;
    public bool CanWater
    {
        get { return _canWater; }
        set { _canWater = value; }
    }

    [SerializeField] private bool _lockedCanWater = false;
    public bool LockedCanWater
    {
        get { return _lockedCanWater; }
        set { _lockedCanWater = value; }
    }

    [Header("PLANT ON TILES SETTINGS")]
    [SerializeField] private bool _canPlantGround = false;
    public bool CanPlantGround
    {
        get { return _canPlantGround; }
        set { _canPlantGround = value; }
    }

    [SerializeField]
    private float stepCooldown = 0.3f;
    private float stepTimer = 0f;
    #endregion

    #region Before Gameloop
    private void Awake()
    {
        
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        //if (SceneUtils.ThisSceneIsGameplayScene(SceneManager.GetActiveScene().ToString()))
        //    onExitToWorldScene(SceneManager.GetActiveScene().ToString());
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameEventsManager.Instance.dataEvents.onExitToWorldScene += onExitToWorldScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameEventsManager.Instance.dataEvents.onExitToWorldScene -= onExitToWorldScene;
    }
   

    private void Update()
    {
        CheckSoundWhenMoving();
        if (SceneManager.GetActiveScene().name.Equals(Loader.Scene.MainMenu.ToString()) ||
            SceneManager.GetActiveScene().name.Equals(Loader.Scene.LoadingScene.ToString()) ||
            SceneManager.GetActiveScene().name.Equals(Loader.Scene.LobbyScene.ToString()) ||
            SceneManager.GetActiveScene().name.Equals(Loader.Scene.CharacterSelectScene.ToString()) ||
            SceneManager.GetActiveScene().name.Equals(Loader.Scene.CutScene.ToString()) ||
            SceneManager.GetActiveScene().name.Equals(Loader.Scene.UIScene.ToString())) return;

        GetTargetTile();
    }
    private void CheckSoundWhenMoving()
    {

        if (playerController.Movement != Vector2.zero)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer > 0f) return;

            Vector3 playerPos = playerController.transform.position;
            Tilemap grassTilemap = null;
            foreach (Tilemap tilemap in _tilemaps)
            {
                if (tilemap.name == "Grass")
                {
                    grassTilemap = tilemap;
                    break;
                }
            }
            Tilemap dirtTilemap = null;
            foreach (Tilemap tilemap in _tilemaps)
            {
                if (tilemap.name == "Ground")
                {
                    dirtTilemap = tilemap;
                    break;
                }
            }

            Vector3Int cellPosOfGrass = grassTilemap.WorldToCell(playerPos);
            Vector3Int cellPosOfGround = dirtTilemap.WorldToCell(playerPos);

            if (grassTilemap.HasTile(cellPosOfGrass))
            {
                AudioManager.Instance.PlaySFX("walk_grass");
            }
            else if (dirtTilemap.HasTile(cellPosOfGround))
            {
                AudioManager.Instance.PlaySFX("walk_Dirt");
            }
            stepTimer = stepCooldown;
        }
        else
        {
            
            stepTimer = 0f;
        }
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("day la scene loaded: " + scene.name);
        if (scene.name.Equals(Loader.Scene.MainMenu.ToString()) ||
            scene.name.Equals(Loader.Scene.LobbyScene.ToString()) ||
            scene.name.Equals(Loader.Scene.LoadingScene.ToString()) ||
            scene.name.Equals(Loader.Scene.CharacterSelectScene.ToString()) ||
            scene.name.Equals(Loader.Scene.UIScene.ToString()) ||
            scene.name.Equals(Loader.Scene.CutScene.ToString())) return;


        onExitToWorldScene(scene.name);
    }
    #endregion

    private void onExitToWorldScene(string sceneName)
    {
        StartCoroutine(WaitForSceneLoaded(sceneName));
    }

    IEnumerator WaitForSceneLoaded(string scene)
    {
        yield return new WaitUntil(() => SceneManager.GetSceneByName(scene).isLoaded);
        string currentSceneName = SceneManager.GetActiveScene().name;

        Debug.Log($"getalltiles");
        
        GetAllTilemaps();

        if (SceneManager.GetActiveScene().name.Equals(Loader.Scene.WorldScene.ToString()))
        {
            _targetTilemap = _tilemaps.LastOrDefault();
            GetTargetTile();
        }
    }

    void GetAllTilemaps()
    {
        GameObject gridObject = GameObject.Find("Grid");

        if (gridObject == null)
        {
            Debug.LogError("No Grid found in the scene!");
            return;
        }

        _tilemaps.Clear();
        Tilemap[] foundTilemaps = gridObject.GetComponentsInChildren<Tilemap>();

        _tilemaps.AddRange(foundTilemaps);
        foreach (Tilemap tilemap in _tilemaps)
        {
            Debug.Log($"Found Tilemap: {tilemap.name}");
        }
    }

    void GetTargetTile()
    {
        // Get mouse position in world coordinates
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mouseWorldPosition.z = 0; // Ensure it's on the correct plane

        // Convert mouse world position to tile position
        _mouseTilePosition = _targetTilemap.WorldToCell(_mouseWorldPosition);

        // Get player position in tile coordinates
        _playerTilePosition = _targetTilemap.WorldToCell(transform.position);

        // Ensure the highlight stays within 1 tile range of the player
        _clampedTilePosition = new Vector3Int(
            Mathf.Clamp(_mouseTilePosition.x, _playerTilePosition.x - TargetRange, _playerTilePosition.x + TargetRange),
            Mathf.Clamp(_mouseTilePosition.y, _playerTilePosition.y - TargetRange, _playerTilePosition.y + TargetRange),
            _mouseTilePosition.z
        );

        // Only update if tile position has changed
        if (_clampedTilePosition != _previousTilePos)
        {
            RefreshTilemapCheck(playerController.noTargetStates.Contains(playerController.CurrentState) ? false : true);
        }
    }

    public void RefreshTilemapCheck(bool showTarget)
    {
        tilemapCheck.Clear();
        if (_targetTilemap == null)
        {
            UnityEngine.Debug.Log("Target Tilemap is not set!");
            return;
        }
        _targetTilemap.SetTile(_previousTilePos, null); // Remove previous highlight

        foreach (Tilemap tilemap in _tilemaps)
        {
            if (tilemap.HasTile(_clampedTilePosition)) 
            {
                tilemapCheck.Add(tilemap);
            }
        }

        if (showTarget)
        {
            _targetTilemap.SetTile(_clampedTilePosition, _targetTile); 
        }
        _previousTilePos = _clampedTilePosition;

        // Check if tile is valid to do something
        CanHoe = CheckCanHoe(_clampedTilePosition);
        CanWater = TileManager.Instance.HoedTilesNetwork.ContainsKey(new NetworkVector3Int(_clampedTilePosition)) && !TileManager.Instance.WateredTilesNetwork.ContainsKey(new NetworkVector3Int(_clampedTilePosition));
        if (tilemapCheck.Count > 0)
        {
            string tilemapName = tilemapCheck[tilemapCheck.Count - 1].name;
            CanPlantGround = (tilemapName == "FarmGround" || tilemapName == "WateredGround");
        }
        else
        {
            CanPlantGround = false;
        }
    }

    private bool CheckCanHoe(Vector3Int pos)
    {
        Tilemap groundTilemap = _tilemaps.Find(x => x.name == "Ground");
        Tilemap grassTilemap = _tilemaps.Find(x => x.name == "Grass");
        if (groundTilemap == null || grassTilemap == null)
        {
            Debug.LogWarning("tilemap to check can hoe not found.");
            return false;
        }
        if (grassTilemap.HasTile(pos)) return false;
        //Vector3Int[] directions =
        //{
        //new Vector3Int(1, 0, 0),
        //new Vector3Int(-1, 0, 0),
        //new Vector3Int(0, 1, 0),
        //new Vector3Int(0, -1, 0),
        //new Vector3Int(1, 1, 0),
        //new Vector3Int(-1, 1, 0),
        //new Vector3Int(1, -1, 0),
        //new Vector3Int(-1, -1, 0)
        //};

        //foreach (var dir in directions)
        //{
        //    if (grassTilemap.HasTile(pos + dir))
        //    {
        //        return false;
        //    }
        //}

        return true;
    }

    public bool CheckHarverst()
    {
        return CropManager.Instance.TryToHarverst(_clampedTilePosition);
    }
    public void UseTool(bool changeFacingDirection)
    {
        if (changeFacingDirection)
        {
            ChangePlayerFacingDirection();
            RefreshTilemapCheck(changeFacingDirection);
            LockClampedPosition();
            LockedCanHoe = CanHoe;
            LockedCanWater = CanWater;
        }

    }

    public void LockClampedPosition()
    {
        _lockedTilePosition = _clampedTilePosition;
    }
    public void PlaceTile(Item item)
    {
        switch (item.itemName)
        {
            default:
                {
                    break;
                }
            case "Hoe":
                {
                    UseHoe(item);
                    break;
                }
            case "WaterCan":
                {
                    UseWaterCan(item);
                    break;
                }
            case "Pickaxe":
                {
                    break;
                }
        }
    }
    private void ChangePlayerFacingDirection()
    {
        if (_clampedTilePosition.x < _playerTilePosition.x)
        {
            playerController.LastMovement = Vector2.left;
            playerController.IsFacingRight = false;
        }
        else if (_clampedTilePosition.x > _playerTilePosition.x)
        {
            playerController.LastMovement = Vector2.right;
            playerController.IsFacingRight = true;
        }

        if (_clampedTilePosition.y > _playerTilePosition.y)
        {
            playerController.LastMovement = Vector2.up;
        }
        else if (_clampedTilePosition.y < _playerTilePosition.y)
        {
            playerController.LastMovement = Vector2.down;
        }
    }

    private void UseHoe(Item item)
    {
        AudioManager.Instance.PlaySFX("shovel");
        if (LockedCanHoe)
        {
            Tilemap targetTilemap = null;
            foreach (Tilemap tilemap in _tilemaps)
            {
                if(tilemap.name == item.tilemap.name)
                {
                    targetTilemap = tilemap;
                    break;
                }
                    
                
            }
            if (!TileManager.Instance.HoedTilesNetwork.ContainsKey(new NetworkVector3Int(_lockedTilePosition)))
            {

                Debug.Log("chat cái nay");
                TileManager.Instance.ModifyTile(_lockedTilePosition, targetTilemap.name, item.ruleTile.name);
            }


        }
    }

    private void UseWaterCan(Item item)
    {
        AudioManager.Instance.PlaySFX("waterCan");
        if (LockedCanWater)
        {
            Tilemap targetTilemap = null;
            foreach (Tilemap tilemap in _tilemaps)
            {
                if (tilemap.name == item.tilemap.name)
                {
                    targetTilemap = tilemap;
                    break;
                }
                    
            }
            if (!TileManager.Instance.WateredTilesNetwork.ContainsKey(new NetworkVector3Int(_lockedTilePosition)))
            {
                TileManager.Instance.ModifyTile(_lockedTilePosition, targetTilemap.name, item.ruleTile.name);

            }
            else
            {
            }
                
        }
        else
        {
        }
    }

    public void SetTile(Item item)
    {
        switch(item.type)
        {
            default:
                break;

            case ItemType.Crop:
                {
                    if (!CanPlantGround) return;
                    GameObject.Find("CropManager").GetComponent<CropManager>().TryModifyCrop(_clampedTilePosition,item.itemName,1);
                    break;
                }        
        }
    }

}
