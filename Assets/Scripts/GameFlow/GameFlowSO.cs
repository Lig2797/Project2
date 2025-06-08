using UnityEngine;

[CreateAssetMenu(fileName = "GameFlowSO", menuName = "Scriptable Objects/GameFlowSO")]
public class GameFlowSO : ScriptableObject
{
    public bool hasChoosenCharacter = false;
    public bool completedFirstCutscene = false;

    public void ResetGameFlow()
    {
        hasChoosenCharacter = false;
        completedFirstCutscene = false;
    }
}
