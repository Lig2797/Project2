using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.Netcode;
using System.Collections;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector cutsceneDirector;

    private void Start()
    {
        //NetworkManager.Singleton.OnServerStarted += () =>
        //{
        //    StartCoroutine(WaitAndAssignPlayer());
        //};

        StartCoroutine(WaitAndAssignPlayer());
    }

    private IEnumerator WaitAndAssignPlayer()
    {
        yield return null;

        GameObject player = FindLocalPlayer();

        if (player != null)
        {
            foreach (var playableAssetOutput in cutsceneDirector.playableAsset.outputs)
            {
                if (playableAssetOutput.streamName == "PlayerTrack")
                {
                    cutsceneDirector.SetGenericBinding(playableAssetOutput.sourceObject, player.GetComponent<Animator>());
                }
            }
            player.gameObject.transform.position = new Vector3(4.371f, 1.154f, 0);
            cutsceneDirector.Play();
        }
    }

    private GameObject FindLocalPlayer()
    {
        foreach (var obj in GameObject.FindObjectsOfType<NetworkBehaviour>())
        {
            if (obj.IsOwner && obj.CompareTag("Player")) 
            {
                return obj.gameObject;
            }
        }

        return null;
    }
}
