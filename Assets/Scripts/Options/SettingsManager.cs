using System.Xml.Serialization;
using UnityEngine;

public class SettingsManager : PersistentSingleton<SettingsManager>
{
    [Header("Reference")]
    public DefaultSettingsSO defaultSettings;

    private void Start()
    {
        LoadData();
        ApplySettings();
    }

    public void ApplySettings()
    {
        SetMasterVolume(defaultSettings.overalVolume);
        SetResolution(defaultSettings.resolutionIndex);
        SetFullScreen(defaultSettings.isFullScreen);
    }

    public void SetMasterVolume(float volume)
    {
        defaultSettings.overalVolume = volume;
        AudioListener.volume = volume;
    }

    public void SetResolution(int resolutionIndex)
    {
        defaultSettings.resolutionIndex = resolutionIndex;

        switch (resolutionIndex)
        {
            case 0:
                Screen.SetResolution(1920, 1080, defaultSettings.isFullScreen);
                break;
            case 1:
                Screen.SetResolution(1280, 720, defaultSettings.isFullScreen);
                break;
            case 2:
                Screen.SetResolution(800, 600, defaultSettings.isFullScreen);
                break;
            default:
                Debug.LogWarning("Invalid resolution index");
                break;
        }
    }

    public void SetFullScreen(bool isFullScreen)
    {
        defaultSettings.isFullScreen = isFullScreen;
        Screen.fullScreen = defaultSettings.isFullScreen;
    }

    public void LoadData()
    {
        SettingsFileHandler.Load(defaultSettings);
    }

    public void SaveData()
    {
        SettingsFileHandler.Save(new SettingsData(defaultSettings.overalVolume,
                                                  defaultSettings.musicVolume,
                                                  defaultSettings.sfxVolume,
                                                  defaultSettings.resolutionIndex,
                                                  defaultSettings.isFullScreen));
    }
}
