using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

public class MultiSceneManger : PersistentSingleton<MultiSceneManger>
{
    [SerializeField]
    private string _worldSceneName = "WorldScene";
    private Scene WorldScene;
    public Scene ActiveSubScene;

    private List<AreaEntrance> allEntranceInWorldScene = new List<AreaEntrance>();

    private GameObject _worldSceneMainCamera;
    private GameObject _worldSceneVirtualCamera;

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

    public void OnChangeScene(Scene SceneToLoad)
    {
        Scene WorldScene = SceneManager.GetSceneByName(_worldSceneName);
        if (SceneToLoad == WorldScene) // back to world
        {
            SetWorldSceneCameraStatus(true);
            FindEntranceToSpawn();
            SetGlobalLightActiveInScene(WorldScene, true);
            CameraController.Instance.RefreshFollowCamera(_worldSceneVirtualCamera.GetComponent<CinemachineVirtualCamera>());
        }
        else
        {
            SetWorldSceneCameraStatus(false);
            CameraController.Instance.SetFollowTarget(); // remove current follow target of current camera
            var newFollowCamera = SceneUtils.FindGameObjectByNameInScene("Virtual Camera", SceneToLoad).GetComponent<CinemachineVirtualCamera>();
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
        _worldSceneMainCamera.SetActive(active);
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
        foreach (AreaEntrance entrance in allEntranceInWorldScene)
        {
            if (entrance.CheckAndSpawnPlayer())
                return;
        }
    }

}
