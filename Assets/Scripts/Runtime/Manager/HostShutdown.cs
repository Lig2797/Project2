using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostShutdown : MonoBehaviour
{
    public void OnShutDownHost()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
