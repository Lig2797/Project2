using UnityEngine;

[System.Serializable]
public class SettingsData
{
    [SerializeField] private float overalVolume;
    [SerializeField] private float musicVolume;
    [SerializeField] private float sfxVolume;
    [SerializeField] private int resolutionIndex;
    [SerializeField] private bool isFullScreen;

    public float OveralVolume
    {
        get => overalVolume;
        private set => overalVolume = Mathf.Clamp01(value);
    }

    public float MusicVolume
    {
        get => musicVolume;
        private set => musicVolume = Mathf.Clamp01(value);
    }

    public float SFXVolume
    {
        get => sfxVolume;
        private set => sfxVolume = Mathf.Clamp01(value);
    }

    public int ResolutionIndex
    {
        get => resolutionIndex;
        private set => resolutionIndex = value;
    }

    public bool IsFullScreen
    {
        get => isFullScreen;
        private set => isFullScreen = value;
    }

    public SettingsData(float overalVolume, float musicVolume, float sfxVolume, int resolutionIndex, bool isFullScreen)
    {
        OveralVolume = overalVolume;
        MusicVolume = musicVolume;
        SFXVolume = sfxVolume;
        ResolutionIndex = resolutionIndex;
        IsFullScreen = isFullScreen;
    }

    public void SetOveralVolume(float volume)
    {
        OveralVolume = volume;
    }
}
