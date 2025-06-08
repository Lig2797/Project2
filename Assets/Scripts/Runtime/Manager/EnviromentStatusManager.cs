using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using Unity.Netcode;

public class EnviromentalStatusManager : NetworkPersistentSingleton<EnviromentalStatusManager>, IDataPersistence
{
    public EnvironmentalStatus eStarus;

    public static event Action<ESeason> ChangeSeasonEvent;

    public static event Action<int> OnTimeIncrease;
    public int minutesToIncrease;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    private void Start()
    {
        
    }

    public bool ChangeSeason()
    {
        switch (eStarus.DateTime.Month, eStarus.DateTime.Day, eStarus.DateTime.Hour, eStarus.DateTime.Minute)
        {
            case (1, 1, 0, 0):
                {
                    eStarus.SetSeasonStatus(ESeason.Spring);
                    return true;
                }
            case (4, 1, 0, 0):
                {
                    eStarus.SetSeasonStatus(ESeason.Summer);
                    return true;
                }
            case (7, 1, 0, 0):
                {
                    eStarus.SetSeasonStatus(ESeason.Autumn);
                    return true;
                }
            case (10, 1, 0, 0):
                {
                    eStarus.SetSeasonStatus(ESeason.Winter);
                    return true;
                }
            default:
                {
                    return false;
                }
        }
    }

    public bool Startday()
    {
        if (eStarus.DateTime.Hour == 6 && eStarus.DateTime.Minute == 0)
        {
            return true;
        }
        return false;
    }

    public bool EndDay()
    {
        if (eStarus.DateTime.Hour == 18 && eStarus.DateTime.Minute == 0)
        {
            return true;
        }
        return false;
    }

    IEnumerator WaitToIncreaseDay()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening) yield return null;

        do
        {
            GameEventsManager.Instance.dateTimeEvents.DateChanged(eStarus.DateTime);
            DayCycleHandler.Instance.MoveSunAndMoon();
            DayCycleHandler.Instance.UpdateLight();
            if (ChangeSeason())
            {
                ChangeSeasonEvent?.Invoke(eStarus.SeasonStatus);
            }
            if (Startday())
            {
                GameEventsManager.Instance.npcEvents.SpawnNpc();
            }
            if (EndDay())
            {
                GameEventsManager.Instance.npcEvents.CallNpcHome();
            }
            yield return new WaitForSeconds(1);
            eStarus.IncreaseDate(minutesToIncrease);
            OnTimeIncrease?.Invoke(minutesToIncrease);
        } while (true);
    }

    public void LoadData(GameData gameData)
    {
        if(!IsHost) return;
        eStarus = gameData.EnviromentData;
        StartCoroutine(WaitToIncreaseDay());
    }

    public void SaveData(ref GameData gameData)
    {
        if (!IsHost) return;

        gameData.SetSeason(eStarus);
    }

}
