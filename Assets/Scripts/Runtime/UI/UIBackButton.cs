using System.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBackButton : MonoBehaviour
{
    public Button backButton;

    private void Start()
    {
        if (backButton == null)
        {
            backButton = GetComponent<Button>();
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        else
        {
            Debug.LogError("Back button is not assigned or found on the GameObject.");
        }
    }

    private void OnBackButtonClicked()
    {
        //SessionManager.Instance.DisInitAndSignOut();
        Loader.Load(Loader.Scene.MainMenu);

        //await SessionManager.Instance.LeaveSession();

        //StartCoroutine(WaitForLeaveSession());
    }

    private IEnumerator WaitForLeaveSession()
    {
        while (SessionManager.Instance.ActiveSession != null)
        {
            yield return null; // Wait until the session is null
        }
        
        yield return new WaitUntil(() => !NetworkManager.Singleton.IsListening);
        Loader.Load(Loader.Scene.MainMenu);
    }
}
