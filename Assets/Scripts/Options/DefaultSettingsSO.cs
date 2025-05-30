using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSettings", menuName = "Game/Default Settings")]
public class DefaultSettingsSO : ScriptableObject
{
    public float overalVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public int resolutionIndex = 0;
    public bool isFullScreen = true;
}
