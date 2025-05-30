using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera followCamera;
    
    void Start()
    {
        GameObject player = FindAnyObjectByType<PlayerController>().gameObject;
        if (player != null)
        {
            followCamera.Follow = player.transform;
        }
        else
        {
            Debug.LogError("PlayerController not found in the scene.");
        }
    }
}