using UnityEngine;

[CreateAssetMenu(fileName = "GameFlowSO", menuName = "Scriptable Objects/GameFlowSO")]
public class GameFlowSO : ScriptableObject
{
    public GameFlowData gameFlowData;

    public void ResetGameFlow()
    {
        gameFlowData = new GameFlowData();
    }
}
