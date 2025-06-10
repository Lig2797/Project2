using UnityEngine;
using UnityEngine.Playables;

public class PlayCutSceneButton : MonoBehaviour
{
    public ECondition eCondition;
    private bool playerInrange = false;
    public PlayableDirector playableDirector;

    public void CheckToPlay(Component sender, object data)
    {
        PlayCutScene();
    }

    private bool HasCompleted(ECondition eCondition)
    {
        switch (eCondition)
        {
            case ECondition.CompletedFirstCutscene:
                return GameFlowManager.Instance.gameFlowSO.gameFlowData.CompletedFirstCutscene;
            case ECondition.CompletedSecondCutscene:
                return GameFlowManager.Instance.gameFlowSO.gameFlowData.CompletedSecondCutscene;
            case ECondition.CompletedThirdCutscene:
                return GameFlowManager.Instance.gameFlowSO.gameFlowData.CompletedThirdCutscene;
            case ECondition.CompletedAllCutscene:
                return GameFlowManager.Instance.gameFlowSO.gameFlowData.CompletedAllCutscene;
            default:
                return false;
        }
    }

    public void PlayCutScene()
    {
        if (HasCompleted(eCondition)) return;

        if (!HasCompleted(eCondition))
        {
            if (playerInrange)
            {
                playableDirector.Play();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInrange = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInrange = true;
        }
    }
}
