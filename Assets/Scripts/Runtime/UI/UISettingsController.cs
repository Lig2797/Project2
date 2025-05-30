using UnityEngine;
using UnityEngine.UIElements;

public class UISettingsController : MonoBehaviour
{
    private VisualElement _optionsPanel;
    private Button _screenSettingButton;
    private Button _soundSettingButton;
    private Button _controlSettingButton;
    private Button _optionsBackButton;

    private VisualElement _screenSettingsPanel;
    private VisualElement _soundSettingsPanel;
    private VisualElement _controlSettingsPanel;

    private DropdownField _resolutionScreenDropdown;
    private Toggle _fullScreenToggle;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _optionsPanel = root.Q<VisualElement>("SettingPanel");
        _screenSettingButton = root.Q<Button>("ScreenSettingButton");
        _soundSettingButton = root.Q<Button>("SoundSettingButton");
        _controlSettingButton = root.Q<Button>("ControlSettingButton");
        _optionsBackButton = root.Q<Button>("OptionsBackButton");

        _screenSettingsPanel = root.Q<VisualElement>("ScreenSetting");
        _soundSettingsPanel = root.Q<VisualElement>("SoundSetting");
        _controlSettingsPanel = root.Q<VisualElement>("ControlSetting");

        _resolutionScreenDropdown = root.Q<DropdownField>("ScreenDropdownField");
        _fullScreenToggle = root.Q<Toggle>("FullScreenToggle");
    }

    private void Start()
    {
        _resolutionScreenDropdown.choices = new System.Collections.Generic.List<string>
        {
            "1920x1080",
            "1280x720",
            "800x600"
        };

        // Set initial values from SettingsManager
        _resolutionScreenDropdown.index = SettingsManager.Instance.CurrentSettings.ResolutionIndex;
        _fullScreenToggle.value = SettingsManager.Instance.CurrentSettings.IsFullScreen;
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.activeUIPanelEvents.onActiveOptions += OnActive;
        GameEventsManager.Instance.activeUIPanelEvents.onDisActivateOptions += OnDisableActive;
        _screenSettingButton.clicked += OnScreenSettingClicked;
        _soundSettingButton.clicked += OnSoundSettingClicked;
        _controlSettingButton.clicked += OnControlSettingClicked;
        _optionsBackButton.clicked += OnOptionsBackButtonClicked;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.activeUIPanelEvents.onActiveOptions -= OnActive;
        GameEventsManager.Instance.activeUIPanelEvents.onDisActivateOptions -= OnDisableActive;
        _screenSettingButton.clicked -= OnScreenSettingClicked;
        _soundSettingButton.clicked -= OnSoundSettingClicked;
        _controlSettingButton.clicked -= OnControlSettingClicked;
        _optionsBackButton.clicked -= OnOptionsBackButtonClicked;
    }

    private void OnScreenSettingClicked()
    {
        ShowOptionsPanel("ScreenSetting");
    }

    private void OnSoundSettingClicked()
    {
        ShowOptionsPanel("SoundSetting");
    }

    private void OnControlSettingClicked()
    {
        ShowOptionsPanel("ControlSetting");
    }

    private void OnOptionsBackButtonClicked()
    {
        OnDisableActive();
        GameEventsManager.Instance.activeUIPanelEvents.OnActiveMainMenu();
    }

    private void OnActive()
    {
        _optionsPanel.AddToClassList("setting-panel-moveleft");
    }

    private void OnDisableActive()
    {
        _optionsPanel.RemoveFromClassList("setting-panel-moveleft");
    }

    private void ShowOptionsPanel(string name)
    {
        _screenSettingsPanel.style.display = DisplayStyle.None;
        _screenSettingsPanel.RemoveFromClassList("screensetting-panel-moveup");
        _soundSettingsPanel.style.display = DisplayStyle.None;
        _soundSettingsPanel.RemoveFromClassList("soundsetting-panel-moveup");
        _controlSettingsPanel.style.display = DisplayStyle.None;
        _controlSettingsPanel.RemoveFromClassList("controlsetting-panel-moveup");

        switch (name)
        {
            case "ScreenSetting":
                _screenSettingsPanel.style.display = DisplayStyle.Flex;
                _screenSettingsPanel.AddToClassList("screensetting-panel-moveup");
                break;
            case "SoundSetting":
                _soundSettingsPanel.style.display = DisplayStyle.Flex;
                _soundSettingsPanel.AddToClassList("soundsetting-panel-moveup");
                break;
            case "ControlSetting":
                _controlSettingsPanel.style.display = DisplayStyle.Flex;
                _controlSettingsPanel.AddToClassList("controlsetting-panel-moveup");
                break;
            default:
                Debug.LogWarning("Unknown settings panel: " + name);
                break;
        }
    }
}
