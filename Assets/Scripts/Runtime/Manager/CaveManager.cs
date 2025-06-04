using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveManager : NetworkPersistentSingleton<CaveManager>
{
    public NetworkDictionary<int, int> caveList;

    private int _currentLocalCaveLevel = 0;
    public int CurrentLocalCaveLevel
    {
        get { return _currentLocalCaveLevel; }
        set
        {
            _currentLocalCaveLevel = value;
            _caveLevelText.text = _currentLocalCaveLevel.ToString();
            _caveLevelBox.SetActive(_currentLocalCaveLevel > 0); 
        }
    }
    [SerializeField] private TextMeshProUGUI _caveLevelText;
    [SerializeField] private GameObject _caveLevelBox;
    private void OnEnable()
    {
        if(caveList == null)
        caveList = new NetworkDictionary<int, int>();
    }

    public void GetUIElement()
    {
        _caveLevelBox = GameObject.Find("CaveLevelBox");
        _caveLevelText = _caveLevelBox.GetComponentInChildren<TextMeshProUGUI>();
    }

    public int GetCaveLevelFromNetwork()
    {
        if (caveList == null || caveList.Count == 0)
        {
            Debug.Log("Cave list is empty.");
            return -1; // Return -1 if the cave list is empty
        }
        foreach (var cave in caveList)
        {
            if (cave.Key == CurrentLocalCaveLevel)
            {
                return cave.Value;
            }
        }
        return -1; // Return -1 if the cave number is not found
    }
    public bool IsHavingOtherPlayerInCave()
    {
        if(CurrentLocalCaveLevel == caveList[caveList.Count - 1])
        {
            return true; 
        }
        return false;
    }
    [ServerRpc(RequireOwnership = false)]
    public void CheckAndAddCaveLevelServerRpc(int caveLevel, int caveNumber)
    {
        if (!caveList.ContainsKey(caveLevel))
        caveList[caveLevel] = caveNumber;
    }

    public void AdjustLocalCaveLevel(int amount)
    {
        CurrentLocalCaveLevel += amount;
    }
    
}
