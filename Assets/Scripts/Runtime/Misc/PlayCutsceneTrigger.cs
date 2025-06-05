using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayCutsceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameEventsManager.Instance.cutsceneEvents.ResumeCutscene();
        }
    }
}
