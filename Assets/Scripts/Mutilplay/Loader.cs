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
        Cutscene,
        LobbyScene,
        LoadingScene,
        CharacterSelectScene,
    }

    private static Scene targetScene;

    public static Scene TargetScene => targetScene;

    public static void Load(Scene targetScene)
    {
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