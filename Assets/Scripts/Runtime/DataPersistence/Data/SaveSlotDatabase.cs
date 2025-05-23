using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveSlotDatabase", menuName = "Save System/Save Slot Database")]
public class SaveSlotDatabase : ScriptableObject
{
    public List<SaveSlotInfo> slots;
}
