using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using Unity.Netcode;

public class UILoadingController : MonoBehaviour
{
    private VisualElement _loadingScreen;
    private VisualElement _progressFill;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        _loadingScreen = root.Q<VisualElement>("LoadingScreen");
        _progressFill = root.Q<VisualElement>("ProgressFill");
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.dataEvents.onDataLoading += LoadSceneWithLoading;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.dataEvents.onDataLoading -= LoadSceneWithLoading;
    }

    private void Start()
    {
        Loader.LoaderCallback();
    }

    private void LoadSceneWithLoading(string sceneName)
    {
        _progressFill.style.width = Length.Percent(0f);
        Debug.Log("Loading with multi: " + Loader.isMultiSceneLoad);
        if(!Loader.isMultiSceneLoad)
            StartCoroutine(LoadAsync(sceneName));
        else
            StartCoroutine(MultiSceneLoadAsync(sceneName));
    }

    private IEnumerator MultiSceneLoadAsync(string sceneName)
    {
        AsyncOperation operation;

        if (sceneName == "WorldScene")
        {
            Debug.Log("Unloading previous subscene: " + MultiSceneManger.Instance.ActiveSubScene.name);
            operation = SceneManager.UnloadSceneAsync(MultiSceneManger.Instance.ActiveSubScene);
        }
        else
        {
            MultiSceneManger.Instance.ActiveSubScene = SceneManager.GetSceneByName(sceneName);
            Debug.Log("subscene after load: " + MultiSceneManger.Instance.ActiveSubScene);
            operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        operation.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            float easedProgress = Mathf.Pow(progress, 0.5f);
            displayedProgress = Mathf.Lerp(displayedProgress, easedProgress, Time.deltaTime * 5f);
            _progressFill.style.width = Length.Percent(displayedProgress * 100f);


            if (operation.progress >= 0.9f && displayedProgress >= 0.98f)
            {
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }


        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("LoadingScene"));
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loadedScene);
        MultiSceneManger.Instance.OnChangeScene(loadedScene);
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        // this is use to Load not main gameplay scenes
        AsyncOperation operation;
        Scene loadingScene = SceneManager.GetActiveScene();
        if (sceneName == "WorldScene")
        {
            
            operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync("UIScene", LoadSceneMode.Additive);
        }
        else
        {
            operation = SceneManager.LoadSceneAsync(sceneName);
        }
        operation.allowSceneActivation = false;
        
        float displayedProgress = 0f;
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            float easedProgress = Mathf.Pow(progress, 0.5f);
            displayedProgress = Mathf.Lerp(displayedProgress, easedProgress, Time.deltaTime * 5f);
            _progressFill.style.width = Length.Percent(displayedProgress * 100f);


            if (operation.progress >= 0.9f && displayedProgress >= 0.98f)
            {
                if (NetworkManager.Singleton.IsHost)
                {

                    //yield return new WaitUntil(() => NetworkManager.Singleton.IsListening);
                }

                if (Loader.TargetScene == Loader.Scene.MainMenu)
                {
                    //yield return new WaitUntil(() => !NetworkManager.Singleton.IsListening);

                    if (Loader.TargetScene == Loader.Scene.WorldScene ||
                    Loader.TargetScene == Loader.Scene.CutScene)
                    {
                        yield return new WaitUntil(() => NetworkManager.Singleton.IsListening);
                    }

                    if (Loader.TargetScene == Loader.Scene.MainMenu)
                    {
                        yield return new WaitUntil(() => !NetworkManager.Singleton.IsListening);
                    }

                }
                

                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        if(sceneName == "WorldScene")
        {
            SceneManager.UnloadSceneAsync(loadingScene);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("WorldScene"));
        }

    }
}
