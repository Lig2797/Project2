using UnityEngine;

[System.Serializable]
public class GameFlowData
{
    [SerializeField] private bool _hasChoosenCharacter;
    [SerializeField] private bool _completedFirstCutscene;
    [SerializeField] private string _lastScene;

    public bool HasChoosenCharacter
    { get { return _hasChoosenCharacter; } }

    public bool CompletedFirstCutscene
    { get { return _completedFirstCutscene; } set { _completedFirstCutscene = value; } }

    public Loader.Scene LastScene
    { get { return ConvertToScene(_lastScene); } }

    public GameFlowData()
    {
        _hasChoosenCharacter = false;
        _completedFirstCutscene = false;
        _lastScene = Loader.Scene.CharacterSelectScene.ToString();
    }

    public GameFlowData(bool hasChoosenCharacter, bool completedFirstCutscene, string lastScene)
    {
        _hasChoosenCharacter = hasChoosenCharacter;
        _completedFirstCutscene = completedFirstCutscene;
        _lastScene = lastScene;
    }

    private Loader.Scene ConvertToScene(string sceneName)
    {
        if (System.Enum.TryParse(sceneName, out Loader.Scene scene))
        {
            return scene;
        }
        else
        {
            Debug.LogError($"Invalid scene name: {sceneName}");
            return Loader.Scene.CharacterSelectScene; // Default value
        }
    }
}
