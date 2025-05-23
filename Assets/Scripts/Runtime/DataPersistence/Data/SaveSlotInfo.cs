using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SaveSlotInfo", menuName = "Save System/Save Slot Info")]
public class SaveSlotInfo : ScriptableObject
{
    public string fileName; 
    public string displayName; 
    public DateTime lastPlayed; 
}
