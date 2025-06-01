using Cinemachine;
using System.Threading.Tasks;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public CinemachineVirtualCamera followCamera;
    
    private async void Start()
    {
        await FindFollowObject();
    }

    public async Task FindFollowObject()
    {
        float timeout = 5f;
        float elapsed = 0f;
        float delay = 0.1f;

        PlayerController playerController = null;

        while (playerController == null && elapsed < timeout)
        {
            await Task.Delay((int)(delay * 1000));
            playerController = FindAnyObjectByType<PlayerController>();
            elapsed += delay;
        }

        if (playerController != null)
        {
            followCamera.Follow = playerController.transform;
            Debug.Log("Camera is now following player.");
        }
        else
        {
            Debug.LogError("Timeout: PlayerController not found in the scene.");
        }
    }

}