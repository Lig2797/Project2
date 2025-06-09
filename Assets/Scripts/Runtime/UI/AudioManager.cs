using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public string masterVolumeParam = "MasterVolume";
    public string musicVolumeParam = "MusicVolume";
    public string sfxVolumeParam = "SFXVolume";

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public List<NamedAudioClip> musicClips;

    [Header("SFX Clips")]
    public List<NamedAudioClip> sfxClips;

    private Dictionary<string, AudioClip> musicDict;
    private Dictionary<string, AudioClip> sfxDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Convert lists to dictionaries for fast lookup
        musicDict = new Dictionary<string, AudioClip>();
        foreach (var item in musicClips)
            musicDict[item.name] = item.clip;

        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var item in sfxClips)
            sfxDict[item.name] = item.clip;
    }

    public void PlayMusic(string name)
    {
        if (musicDict.TryGetValue(name, out var clip))
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip '{name}' not found!");
        }
    }

    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out var clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX clip '{name}' not found!");
        }
    }

    public void SetMasterVolume(float volume) => SetVolume(masterVolumeParam, volume);
    public void SetMusicVolume(float volume) => SetVolume(musicVolumeParam, volume);
    public void SetSFXVolume(float volume) => SetVolume(sfxVolumeParam, volume);

    private void SetVolume(string param, float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(param, dB);
    }

    public float GetVolume(string param)
    {
        if (audioMixer.GetFloat(param, out float dB))
            return Mathf.Pow(10f, dB / 20f);
        return 1f;
    }
}

[System.Serializable]
public class NamedAudioClip
{
    public string name;
    public AudioClip clip;
}
