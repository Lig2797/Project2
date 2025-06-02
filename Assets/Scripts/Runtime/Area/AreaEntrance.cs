using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaEntranceType
{
    Mine,
    Town,
    HouseDoor,
    Other
}
public class AreaEntrance : MonoBehaviour
{
    [SerializeField] private string transitionName;
    [SerializeField] private AreaEntranceType entranceType;

    private void Start() 
    {
        if (transitionName == SceneManagement.SceneTransitionName)
        {

            if (entranceType == AreaEntranceType.Mine)
                PlayerController.LocalInstance.transform.position = this.transform.position + Vector3.down;
            else if(entranceType == AreaEntranceType.HouseDoor)
                PlayerController.LocalInstance.transform.position = this.transform.position + Vector3.up;

            else
                PlayerController.LocalInstance.transform.position = this.transform.position;

            PlayerController.LocalInstance.GetCameraFollow();
            PlayerController.LocalInstance.CanMove = true;
            UI_Fade.Instance.FadeToClear();
            UI_Fade.Instance.gameObject.SetActive(false);
        }
    }
}
