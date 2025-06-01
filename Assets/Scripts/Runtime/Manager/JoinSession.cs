using UnityEngine;
using UnityEngine.UI;

public class JoinSession : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    private void Awake()
    {
        foreach (Button button in buttons)
        {     
            button.onClick.AddListener(OnJoinSessionClicked);
        }
    }

    private void OnJoinSessionClicked()
    {
        Loader.Load(Loader.Scene.Cutscene);
    }
}
