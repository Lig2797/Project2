using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameStartLoader : MonoBehaviour
{
    [SerializeField] private string worldSceneName = "WorldScene";

    private void Start()
    {
        Loader.Load(Loader.Scene.WorldScene);
        
        //StartCoroutine(LoadWorldScene());
    }

    private IEnumerator LoadWorldScene()
    {
        // 🔒 Store the current scene BEFORE changing active scene
        Scene bootstrapScene = SceneManager.GetActiveScene();

        // Load WorldScene additively
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(worldSceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => asyncLoad.isDone);

        // Get and set WorldScene as active
        Scene worldScene = SceneManager.GetSceneByName(worldSceneName);
        if (worldScene.IsValid())
        {
            SceneManager.SetActiveScene(worldScene);
            Debug.Log("World scene loaded and set as active.");

            //get these references from the world scene on first load to gameplay
            GameObject mainWorldSceneCamera = SceneUtils.FindGameObjectWithTagInScene("MainCamera", worldScene);
            GameObject WorldSceneVirtualCamera = SceneUtils.FindGameObjectByNameInScene("Virtual Camera", worldScene);
            MultiSceneManger.Instance.SetWorldSceneCameraObject(mainWorldSceneCamera, WorldSceneVirtualCamera);
            MultiSceneManger.Instance.SaveWorldSceneRef(worldScene);

            // ✅ Now unload the original (cached) bootstrap scene
            if (bootstrapScene.name != worldSceneName)
            {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(bootstrapScene);
                yield return new WaitUntil(() => unloadOp.isDone);
                Debug.Log("Bootstrap scene unloaded.");
            }
        }
    }


}
