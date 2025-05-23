using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class UIMainMenuController : MonoBehaviour
{
    //Main menu
    private VisualElement _mainMenuContainer;
    private VisualElement _gameTitleLabel;
    private Button _singleplayerButton;
    private Button _multiplayerButton;
    private Button _optionsButton;
    private Button _creditsButton;
    private Button _quitButton;

    //Save file panel
    private VisualElement _saveFilePanel;
    private ListView _saveFileList;
    private Button _newGameButton;
    private Button _loadGameButton;
    private Button _saveFileBackButton;

    //Option panel
    private VisualElement _optionsPanel;
    private Button _optionsBackButton;

    private LoadingManager _loadingManager;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _mainMenuContainer = root.Q<VisualElement>("MainMenuContainer");
        _gameTitleLabel = root.Q<Label>("GameTitleLabel");
        _singleplayerButton = root.Q<Button>("SingleplayerButton");
        _multiplayerButton = root.Q<Button>("MultiplayerButton");
        _optionsButton = root.Q<Button>("OptionsButton");
        _creditsButton = root.Q<Button>("CreditsButton");
        _quitButton = root.Q<Button>("QuitButton");

        _saveFilePanel = root.Q<VisualElement>("SaveFilePanel");
        _saveFileList = root.Q<ListView>("SaveFileList");
        _newGameButton = root.Q<Button>("NewGameButton");
        _loadGameButton = root.Q<Button>("LoadGameButton");
        _saveFileBackButton = root.Q<Button>("SaveFileBackButton");

        _optionsPanel = root.Q<VisualElement>("SettingPanel");
        _optionsBackButton = root.Q<Button>("OptionsBackButton");
        
        _loadingManager = FindObjectOfType<LoadingManager>();
    }

    private void Start()
    {
        StartCoroutine(AnimateTitleLoop());
    }

    private void OnEnable()
    {
        _singleplayerButton.clicked += OnSingleplayerButtonClicked;
        _optionsButton.clicked += OnOptionsButtonClicked;
        _quitButton.clicked += () =>
        {
            Application.Quit();
        };

        _newGameButton.clicked += OnNewGameButtonClicked;
        
        _saveFileBackButton.clicked += () =>
        {
            _mainMenuContainer.RemoveFromClassList("mainmenu-panel-moveleft");
            _saveFilePanel.RemoveFromClassList("savefile-panel-moveleft");
        };
        
        _optionsBackButton.clicked += () =>
        {
            _mainMenuContainer.RemoveFromClassList("mainmenu-panel-moveleft");
            _optionsPanel.RemoveFromClassList("setting-panel-moveleft");
        };
    }

    private void OnDisable()
    {
        _singleplayerButton.clicked -= OnSingleplayerButtonClicked;
        _optionsButton.clicked -= OnOptionsButtonClicked;
        _quitButton.clicked -= () =>
        {
            Application.Quit();
        };

        _newGameButton.clicked -= OnNewGameButtonClicked;
        
        _saveFileBackButton.clicked -= () =>
        {
            _mainMenuContainer.RemoveFromClassList("mainmenu-panel-moveleft");
            _saveFilePanel.RemoveFromClassList("savefile-panel-moveleft");
        };

        _optionsBackButton.clicked -= () =>
        {
            _mainMenuContainer.RemoveFromClassList("mainmenu-panel-moveleft");
            _optionsPanel.RemoveFromClassList("setting-panel-moveleft");
        };
    }

    private void OnSingleplayerButtonClicked()
    {
        _mainMenuContainer.AddToClassList("mainmenu-panel-moveleft");
        _saveFilePanel.AddToClassList("savefile-panel-moveleft");
    }

    private void OnOptionsButtonClicked()
    {
        _mainMenuContainer.AddToClassList("mainmenu-panel-moveleft");
        _optionsPanel.AddToClassList("setting-panel-moveleft");
    }

    private void OnNewGameButtonClicked()
    {
        _mainMenuContainer.style.display = DisplayStyle.None;
        _saveFilePanel.style.display = DisplayStyle.None;
        _loadingManager.LoadSceneWithLoading("WorldScene");
    }

    private IEnumerator AnimateTitleLoop()
    {
        while (true)
        {
            _gameTitleLabel.AddToClassList("gametitlelable--zoom");
            yield return new WaitForSeconds(1f);
            _gameTitleLabel.RemoveFromClassList("gametitlelable--zoom");
            yield return new WaitForSeconds(1f);
        }
    }
}
