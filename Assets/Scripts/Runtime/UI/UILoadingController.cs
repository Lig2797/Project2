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
            operation = SceneManager.UnloadSceneAsync(MultiSceneManger.Instance.ActiveSubScene);
        }
        else
        {
            MultiSceneManger.Instance.ActiveSubScene = SceneManager.GetSceneByName(sceneName);
            operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        //operation.allowSceneActivation = false;

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
            }

            yield return null;
        }

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loadedScene);
        MultiSceneManger.Instance.OnChangeScene(loadedScene);
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        // this is use to Load not main gameplay scenes
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        if (sceneName == "WorldScene")
        {
            SceneManager.LoadSceneAsync("UIScene", LoadSceneMode.Additive);
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

        
    }
}
