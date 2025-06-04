using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenu,
        WorldScene,
        CutScene,
        LobbyScene,
        LoadingScene,
        CharacterSelectScene,
    }

    private static Scene lastScene;

    public static Scene LastScene => lastScene;

    private static Scene targetScene;

    public static Scene TargetScene => targetScene;

    private static Scene ConvertSceneNameToEnum(string sceneName)
    {
        foreach (Scene scene in Scene.GetValues(typeof(Scene)))
        {
            if (scene.ToString() == sceneName)
            {
                return scene;
            }
        }

        Debug.LogWarning($"Scene name '{sceneName}' is not defined in Loader.Scene enum. Defaulting to MainMenu.");
        return Scene.MainMenu;
    }

    public static void Load(Scene targetScene)
    {
        Loader.lastScene = ConvertSceneNameToEnum(SceneManager.GetActiveScene().name);
        Loader.targetScene = targetScene;
        SceneManager.LoadSceneAsync(Scene.LoadingScene.ToString());
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        GameEventsManager.Instance.dataEvents.OnDataLoading(targetScene.ToString());
    }

}