using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEntrance : MonoBehaviour
{
    [SerializeField] private string transitionName;

    private void Start() 
    {
        if (transitionName == SceneManagement.SceneTransitionName)
        {
            PlayerController.LocalInstance.transform.position = this.transform.position;
            PlayerController.LocalInstance.CanMove = true;
            UI_Fade.Instance.FadeToClear();
            UI_Fade.Instance.gameObject.SetActive(false);
        }
    }
}
