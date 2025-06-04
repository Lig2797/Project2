using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : PersistentSingleton<GameFlowManager>, IDataPersistence
{
    [Header("Reference")]
    public GameFlowSO gameFlowSO;

    public void LoadLastSceneFlow()
    {
        Loader.Load(gameFlowSO.lastScene);
    }

    public void HasChoosenCharacte(bool hasChoosenCharacter)
    {
        gameFlowSO.hasChoosenCharacter = hasChoosenCharacter;
    }

    public void SetCompletedFirstCutscene(bool completed)
    {
        gameFlowSO.completedFirstCutscene = completed;
    }

    public void SetLastScene(Loader.Scene scene)
    {
        gameFlowSO.lastScene = scene;
    }

    public void LoadData(GameData data)
    {
        gameFlowSO.hasChoosenCharacter = data.GameFlowData.HasChoosenCharacter;
        gameFlowSO.completedFirstCutscene = data.GameFlowData.CompletedFirstCutscene;
        gameFlowSO.lastScene = data.GameFlowData.LastScene;
    }

    public void SaveData(ref GameData data)
    {
        data.SetGameFlowData(new GameFlowData (gameFlowSO.hasChoosenCharacter,
                                               gameFlowSO.completedFirstCutscene,
                                               SceneManager.GetActiveScene().name));
    }
}
