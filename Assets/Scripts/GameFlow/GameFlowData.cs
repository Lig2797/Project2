using UnityEngine;

[System.Serializable]
public class GameFlowData
{
    [SerializeField] private bool _hasChoosenCharacter;
    [SerializeField] private bool _completedFirstCutscene;
    [SerializeField] private bool _completedSecondCutscene;
    [SerializeField] private bool _hasOpenedPlayerHouse;
    [SerializeField] private bool _completedThirdCutscene;
    [SerializeField] private bool _completedAllCutscene;

    public bool HasChoosenCharacter
    { get { return _hasChoosenCharacter; } }

    public bool CompletedFirstCutscene
    { get { return _completedFirstCutscene; } }

    public bool CompletedSecondCutscene
    { get { return _completedSecondCutscene; } }

    public bool HasOpenedPlayerHouse
    { get { return _hasOpenedPlayerHouse; } }

    public bool CompletedThirdCutscene
    { get { return _completedThirdCutscene; } }

    public bool CompletedAllCutscene
    { get { return _completedAllCutscene; } }

    public GameFlowData()
    {
        _hasChoosenCharacter = false;
        _completedFirstCutscene = false;
        _completedSecondCutscene = false;
        _completedThirdCutscene = false;
        _completedAllCutscene = false;
    }

    public GameFlowData(bool hasChoosenCharacter, bool completedFirstCutscene, bool completeSecondCutscene, bool completedThirdCutscene, bool completedAllCutscene)
    {
        _hasChoosenCharacter = hasChoosenCharacter;
        _completedFirstCutscene = completedFirstCutscene;
        _completedSecondCutscene = completeSecondCutscene;
        _completedThirdCutscene = completedThirdCutscene;
        _completedAllCutscene = completedAllCutscene;

    }

    public void SetHasChoosenCharacter(bool hasChoosenCharacter)
    {
        _hasChoosenCharacter = hasChoosenCharacter;
    }

    public void SetCompletedFirstCutscene(bool completedFirstCutscene)
    {
        _completedFirstCutscene = completedFirstCutscene;
    }

    public void SetCompletedSecondCutscene(bool completedSecondCutscene)
    {
        _completedSecondCutscene = completedSecondCutscene;
    }

    public void SetHasOpendPlayerHouse(bool hasOpenedPlayerHouse)
    {
        _hasOpenedPlayerHouse = hasOpenedPlayerHouse;
    }

    public void SetCompletedThirdCutscene(bool completedThirdCutscene)
    {
        _completedThirdCutscene = completedThirdCutscene;
    }

    public void SetCompletedAllCutscene(bool completedAllCutscene)
    {
        _completedAllCutscene = completedAllCutscene;
    }
}
