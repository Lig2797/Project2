using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System.IO;
using Unity.Netcode;
using System;
using System.Runtime.CompilerServices;


public class DataPersistenceManager : Singleton<DataPersistenceManager>
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    //private HashSet<IDataPersistence> readyObjects = new();

    private void Start()
    {
        InitializeDataHandler();
        dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    private void InitializeDataHandler()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    public void UpdateFileName(string newFileName)
    {
        fileName = newFileName;
        InitializeDataHandler();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame(bool isHostLoad, Action onFinished = null)
    {
        StartCoroutine(WaitAndLoad(isHostLoad, onFinished));
    }

    private IEnumerator WaitAndLoad(bool isHostLoad, Action onFinished)
    {
        // 🔁 Wait until the NetworkManager is fully initialized
        yield return new WaitUntil(() => NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening);

        // 🔁 Optional: wait an extra frame or two to ensure all NetworkObjects are spawned
        yield return null;
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
            yield break;
        }
        // 🔁 Wait until all IDataPersistence objects are found
        yield return new WaitUntil(() => dataPersistenceObjects.All(obj => (obj as NetworkBehaviour)?.IsSpawned == true));
        Debug.Log("All PersistentDataObjects are fully spawned.");
        // ✅ Now all components are ready
        if (isHostLoad)
        {
            foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
            {
                dataPersistenceObject.LoadData(gameData);
            }
        }
        else
        {
            SyncWorldDataToPlayerServerRpc();
        }


        onFinished?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SyncWorldDataToPlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;

        var rpcParamsForClient = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { clientId }
            }
        };
        SyncWorldDataToPlayerClientRpc(rpcParamsForClient);
    }

    [ClientRpc]
    private void SyncWorldDataToPlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        TileManager.Instance.SyncTileOnLateJoin();
        ItemWorldManager.Instance.SyncItemWorldOnLateJoin();
        CropManager.Instance.SyncCropsOnLateJoin();

    }
    public void SaveGame(bool isServerSave) 
    {
        if (isServerSave)
        {
            foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
            {
                dataPersistenceObject.SaveData(ref gameData);
            }
        }
        dataHandler.Save(gameData);

        //CaptureScreenshot();
    }

    public void RemoveData()
    {
        dataHandler.DeleteData();

        string screenshotPath = Path.Combine(Application.persistentDataPath, fileName + "_screenshot.png");
        if (File.Exists(screenshotPath))
        {
            File.Delete(screenshotPath);
        }

        NewGame();
        Debug.Log("Data and screenshot removed.");
    }

    public void OnApplicationQuit()
    {
        //SaveGame();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void OnPlayerLoad(Component sender, object data)
    {
        var player = sender.GetComponent<PlayerController>();
        if(player == null)
        {
            Debug.Log("PlayerController is null");
            return;
        }
        LoadGame((bool)data, () => player.StartToLoad(gameData));
    }
    public void OnPlayerSave(Component sender, object data)
    {
        var player = sender.GetComponent<PlayerController>();
        if (player == null)
        {
            Debug.Log("PlayerController is null");
        }
        else
        {
            player.StartToSave(ref gameData);
        }


        SaveGame((bool)data);
    }

    //private void CaptureScreenshot()
    //{
    //    string screenshotPath = Path.Combine(Application.persistentDataPath, fileName + "_screenshot.png");

    //    // Tạo RenderTexture với độ phân giải của màn hình
    //    RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
    //    Camera.main.targetTexture = renderTexture;
    //    Camera.main.Render();

    //    // Chuyển RenderTexture thành Texture2D
    //    RenderTexture.active = renderTexture;
    //    Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    //    screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    //    screenshot.Apply();

    //    // Gỡ RenderTexture và giải phóng tài nguyên
    //    Camera.main.targetTexture = null;
    //    RenderTexture.active = null;
    //    Destroy(renderTexture);

    //    // Lưu ảnh dưới dạng PNG
    //    byte[] bytes = screenshot.EncodeToPNG();
    //    File.WriteAllBytes(screenshotPath, bytes);
    //}
}
