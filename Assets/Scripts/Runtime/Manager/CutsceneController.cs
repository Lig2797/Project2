using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.Netcode;
using System.Collections;

public class CutsceneController : MonoBehaviour
{
    private bool hasPlayerAssigned = false;
    [SerializeField] private PlayableDirector cutsceneDirector;

    private void OnEnable()
    {
        GameEventsManager.Instance.playerEvents.onPlayerSpawned += AssignPlayer;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.playerEvents.onPlayerSpawned -= AssignPlayer;
    }

    private void Start()
    {
        if (!hasPlayerAssigned)
        {
            PlayerController player = PlayerController.LocalInstance;
            if (player != null)
            {
                AssignPlayer(player);
            }
            else
            {
                Debug.LogWarning("No PlayableDirector found in the scene. Cutscene cannot be played.");
            }
        }
    }

    private void AssignPlayer(PlayerController player)
    {
        if (!NetworkManager.Singleton.IsHost) return;

        foreach (var playableAssetOutput in cutsceneDirector.playableAsset.outputs)
        {
            if (playableAssetOutput.streamName == "PlayerTrack")
            {
                cutsceneDirector.SetGenericBinding(playableAssetOutput.sourceObject, player.GetComponent<Animator>());
            }
        }
        player.gameObject.transform.position = new Vector3(4.371f, 1.154f, 0);
        cutsceneDirector.Play();
        hasPlayerAssigned = true;
    }
}
