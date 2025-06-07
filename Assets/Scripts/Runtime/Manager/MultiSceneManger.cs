using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

public class MultiSceneManger : PersistentSingleton<MultiSceneManger>
{
    private Scene WorldScene;
    public Scene ActiveSubScene;
    [property: SerializeField]
    public string ActiveSubSceneName => ActiveSubScene.name;

    private List<AreaEntrance> allEntranceInWorldScene = new List<AreaEntrance>();

    public GameObject _worldSceneMainCamera;
    public GameObject _worldSceneVirtualCamera;

    //public void EnterSubScene(string sceneName)
    //{
    //    StartCoroutine(LoadSubScene(sceneName));
    //}

    //public void ExitToWorld()
    //{
    //    StartCoroutine(UnloadSubScene());
    //}

    //private IEnumerator LoadSubScene(string sceneName)
    //{
    //    if (_activeSubScene.IsValid())
    //    {
    //        yield return UnloadSubScene();
    //    }
    //    // Load the subscene

    //    SetGlobalLightActiveInScene(WorldScene, false); // disable global light in world scene if entering subscene
    //    AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    //    yield return new WaitUntil(() => loadOp.isDone);

    //    Scene loadedScene = SceneManager.GetSceneByName(sceneName);
    //    if (loadedScene.IsValid())
    //    {
    //        _activeSubScene = loadedScene;
    //        SceneManager.SetActiveScene(loadedScene);
    //        OnChangeScene(SceneManager.GetSceneByName(sceneName));
    //    }


    //}


    //private IEnumerator UnloadSubScene()
    //{
    //    if (!_activeSubScene.IsValid()) yield break;

    //    AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(_activeSubScene);
    //    yield return new WaitUntil(() => unloadOp.isDone);

    //    _activeSubScene = new Scene();

    //    Scene worldScene = SceneManager.GetSceneByName(_worldSceneName);
    //    SceneManager.SetActiveScene(worldScene);
    //    OnChangeScene(worldScene);

    //}

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void SetGlobalLightActiveInScene(Scene scene, bool active)
    {
        if (!scene.IsValid() || !scene.isLoaded) return;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            var globalLights = root.GetComponentsInChildren<UnityEngine.Rendering.Universal.Light2D>(true);
            foreach (var light in globalLights)
            {
                if (light.lightType == UnityEngine.Rendering.Universal.Light2D.LightType.Global)
                {
                    light.gameObject.SetActive(active);
                }
            }
        }
    }

    public void OnExitToWorldScene(Scene SceneToLoad)
    {
        SetWorldSceneCameraStatus(true);
        FindEntranceToSpawn();
        SetGlobalLightActiveInScene(WorldScene, true);
        CameraController.Instance.RefreshFollowCamera(_worldSceneVirtualCamera.GetComponent<CinemachineVirtualCamera>());

    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == Loader.Scene.MainMenu.ToString() ||
            scene.name == Loader.Scene.CutScene.ToString() ||
            scene.name == Loader.Scene.CharacterSelectScene.ToString() ||
            scene.name == Loader.Scene.LobbyScene.ToString() ||
            scene.name == Loader.Scene.UIScene.ToString() ||
            scene.name == Loader.Scene.LoadingScene.ToString())
            return;
        if (scene.name == Loader.Scene.WorldScene.ToString())
        {
            GameObject mainWorldSceneCamera = SceneUtils.FindGameObjectWithTagInScene("MainCamera", scene);
            GameObject worldSceneVirtualCamera = SceneUtils.FindGameObjectByNameInScene("Virtual Camera", scene);
            
            SetWorldSceneCameraObject(mainWorldSceneCamera, worldSceneVirtualCamera);
            SaveWorldSceneRef(scene);

            Debug.Log("World scene references saved after load.");
        }
        else // load to subscene
        {
            Debug.Log("Did OnSwitchSceneWhileGameplay");
            SetWorldSceneCameraStatus(false);
            SetGlobalLightActiveInScene(WorldScene, false);
            var newFollowCamera = SceneUtils.FindGameObjectByNameInScene("Virtual Camera", scene).GetComponent<CinemachineVirtualCamera>();
            CameraController.Instance.RefreshFollowCamera(newFollowCamera);
        }
    }
    public void SetWorldSceneCameraObject(GameObject cameraObject, GameObject virtualCameraObject)
    {
        _worldSceneMainCamera = cameraObject;
        _worldSceneVirtualCamera = virtualCameraObject;
    }

    public void SaveWorldSceneRef(Scene worldScene)
    {
        WorldScene = worldScene;
    }

    public void SetWorldSceneCameraStatus(bool active)
    {
        if(_worldSceneMainCamera != null)
        _worldSceneMainCamera.SetActive(active);
        if(_worldSceneVirtualCamera != null)
        _worldSceneVirtualCamera.SetActive(active);
    }

    public void SubscribeToEntranceList(AreaEntrance entrance)
    {
        allEntranceInWorldScene.Add(entrance);
    }

    public void UnsubscribeFromEntranceList(AreaEntrance entrance)
    {
        allEntranceInWorldScene.Remove(entrance);
    }

    private void FindEntranceToSpawn()
    {
        if(allEntranceInWorldScene == null)
        {
            Debug.Log("No entrance registered");
            return;
        }
        foreach (AreaEntrance entrance in allEntranceInWorldScene)
        {
            if (entrance.CheckAndSpawnPlayer())
                return;
        }
    }
    
}
