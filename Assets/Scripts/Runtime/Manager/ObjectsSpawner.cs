using UnityEngine;

public class ObjectsSpawner : MonoBehaviour
{
    [SerializeField] private string knotName;
    [SerializeField] private GameObject objectPrefab;

    private void Start()
    {
        CheckSpawn();
    }

    private void CheckSpawn()
    {
        if (GameFlowManager.Instance.gameFlowSO.gameFlowData.CompletedThirdCutscene)
        {
            GameObject npcGO = Instantiate(objectPrefab, this.gameObject.transform.position, Quaternion.identity);
            DialogueTrigger dialogueTrigger = npcGO.GetComponentInChildren<DialogueTrigger>();
            if (dialogueTrigger != null)
            {
                dialogueTrigger.enabled = true;
                dialogueTrigger.SetKnotName(knotName);
            }
        }    
    }    
}
