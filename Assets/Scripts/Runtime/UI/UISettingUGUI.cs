using UnityEngine;
using UnityEngine.UI;

public class UISettingUGUI : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Load saved volume or default 1
        masterSlider.value = AudioManager.Instance.GetVolume("MasterVolume");
        musicSlider.value = AudioManager.Instance.GetVolume("MusicVolume");
        sfxSlider.value = AudioManager.Instance.GetVolume("SFXVolume");

        // Add listeners
        masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
    }
}
