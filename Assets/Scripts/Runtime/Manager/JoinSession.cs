using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class JoinSession : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    ISessionInfo sessionInfo;

    private void Awake()
    {
        foreach (Button button in buttons)
        {     
            button.onClick.AddListener(OnJoinSessionClicked);
        }
    }

    private void OnEnable()
    {

    }

    private void SetSessionInfo(ISessionInfo sessionInfo)
    {
        this.sessionInfo = sessionInfo;
    }

    private void OnJoinSessionClicked()
    {
        Loader.Load(Loader.Scene.Cutscene);
    }
}
