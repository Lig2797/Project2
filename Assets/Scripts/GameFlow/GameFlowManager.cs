using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : PersistentSingleton<GameFlowManager>, IDataPersistence
{
    [Header("Reference")]
    public GameFlowSO gameFlowSO;

    public void HasChoosenCharacte(bool hasChoosenCharacter)
    {
        gameFlowSO.gameFlowData.SetHasChoosenCharacter(hasChoosenCharacter);
    }

    public void SetCompletedFirstCutscene(bool completed)
    {
        gameFlowSO.gameFlowData.SetCompletedFirstCutscene(completed);
    }

    public void SetCompletedSecondCutscene(bool completed)
    {
        gameFlowSO.gameFlowData.SetCompletedSecondCutscene(completed);
    }

    public void LoadData(GameData data)
    {
        if (!gameFlowSO.gameFlowData.HasChoosenCharacter || gameFlowSO.gameFlowData.CompletedFirstCutscene) return;

        gameFlowSO.gameFlowData = data.GameFlowData;
    }

    public void SaveData(ref GameData data)
    {
        data.SetGameFlowData(gameFlowSO.gameFlowData);

        gameFlowSO.ResetGameFlow();
    }
}
