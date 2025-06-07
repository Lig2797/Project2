using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaEntranceType
{
    Mine,
    Town,
    HouseInDoor,
    HouseOutDoor,
    Other
}
public class AreaEntrance : MonoBehaviour
{
    [SerializeField] private string transitionName;
    [SerializeField] private AreaEntranceType entranceType;

    private void Start() 
    {
        CheckAndSpawnPlayer();
    }

    private void OnEnable()
    {
        MultiSceneManger.Instance.SubscribeToEntranceList(this);
    }

    private void OnDisable()
    {
        MultiSceneManger.Instance.UnsubscribeFromEntranceList(this);
    }

    public bool CheckAndSpawnPlayer()
    {
        if (transitionName == SceneManagement.SceneTransitionName)
        {
            Debug.Log("found entrance and start to set position: " + transform.position + Vector3.up);
            if (entranceType == AreaEntranceType.Mine)
            {
                PlayerController.LocalInstance.transform.position = this.transform.position + Vector3.down;
            }
            else if (entranceType == AreaEntranceType.HouseInDoor)
            {
                PlayerController.LocalInstance.transform.position = this.transform.position + Vector3.up;
            }
            else if (entranceType == AreaEntranceType.HouseOutDoor)
            {
                PlayerController.LocalInstance.transform.position = this.transform.position + Vector3.down;
            }
            else
            {
                PlayerController.LocalInstance.transform.position = this.transform.position;
            }

            PlayerController.LocalInstance.CanMove = true;
            //UI_Fade.Instance.FadeToClear();
            //UI_Fade.Instance.gameObject.SetActive(false);

            return true;
        }

        return false;
    }
}
