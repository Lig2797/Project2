using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : PersistentSingleton<GameFlowManager>, IDataPersistence
{
    [Header("Reference")]
    public GameFlowSO gameFlowSO;

    public void HasChoosenCharacte(bool hasChoosenCharacter)
    {
        gameFlowSO.hasChoosenCharacter = hasChoosenCharacter;
    }

    public void SetCompletedFirstCutscene(bool completed)
    {
        gameFlowSO.completedFirstCutscene = completed;
    }

    public void LoadData(GameData data)
    {
        //if (gameFlowSO.hasChoosenCharacter) return;

        gameFlowSO.hasChoosenCharacter = data.GameFlowData.HasChoosenCharacter;
        gameFlowSO.completedFirstCutscene = data.GameFlowData.CompletedFirstCutscene;
    }

    public void SaveData(ref GameData data)
    {
        data.SetGameFlowData(new GameFlowData (gameFlowSO.hasChoosenCharacter,
                                               gameFlowSO.completedFirstCutscene,
                                               SceneManager.GetActiveScene().name));

        gameFlowSO.ResetGameFlow();
    }
}
