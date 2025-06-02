using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostShutdown : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Button is not assigned in the inspector.");
        }
    }

    private void OnButtonClick()
    {
        DataPersistenceManager.Instance.SaveGame();
        DataPersistenceManager.Instance.CaptureScreenshot();
        NetworkManager.Singleton.Shutdown();
    }
}
