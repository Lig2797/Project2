using Cinemachine;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    private bool hasFollowTarget = false;
    public CinemachineVirtualCamera followCamera;

    private void OnEnable()
    {
        GameEventsManager.Instance.playerEvents.onPlayerSpawned += SetFollowTarget;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.playerEvents.onPlayerSpawned -= SetFollowTarget;
    }

    private void Start()
    {
        if (!hasFollowTarget)
        {
            PlayerController player = PlayerController.LocalInstance;
            if (player != null)
            {
                SetFollowTarget(player);
            }
            else
            {
                Debug.LogWarning("No PlayerController found in the scene. Camera will not follow any target.");
            }
        }
    }

    private void SetFollowTarget(PlayerController player)
    {
        followCamera.Follow = player.transform;
        hasFollowTarget = true;
    }    
}