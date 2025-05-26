using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    [Header("Reference")]
    public DefaultSettingsSO defaultSettings;

    public SettingsData CurrentSettings { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    public void LoadSettings()
    {
        CurrentSettings = SettingsFileHandler.Load(defaultSettings);
        ApplySettings();
    }

    public void SaveSettings()
    {
        SettingsFileHandler.Save(CurrentSettings);
    }

    public void ApplySettings()
    {
        AudioListener.volume = CurrentSettings.OveralVolume;
        Screen.fullScreen = CurrentSettings.IsFullScreen;

        // Resolution áp dụng sau nếu cần
    }

    public void SetMasterVolume(float volume)
    {
        CurrentSettings.SetOveralVolume(volume);
        AudioListener.volume = volume;
    }

    // Các setter tương tự cho MusicVolume, SFXVolume, Fullscreen...
}
