using UnityEngine;

public enum ECondition
{
    CompletedFirstCutscene,
    CompletedSecondCutscene,
    CompletedThirdCutscene,
    CompletedAllCutscene,
}

public class ObjectsSpawner : MonoBehaviour
{
    public enum EObjDialogue
    {
        Default,
        Npc
    }   
    
    public enum ENpcAction
    {
        None,
        Chopping
    }   
    
    public EObjDialogue eObjDialogue;
    public ENpcAction action;
    public ECondition spawnCondition;
    [SerializeField] private string knotName;
    [SerializeField] private GameObject objectPrefab;

    private void OnEnable()
    {
        GameEventsManager.Instance.objectEvents.onSpawnObject += CheckSpawn;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.objectEvents.onSpawnObject -= CheckSpawn;
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

    private void CheckSpawn()
    {
        if (HasCompleted(spawnCondition)) return;

        if (!HasCompleted(spawnCondition))
        {
            if (eObjDialogue == EObjDialogue.Default)
            {
                DialogueTrigger dlTrigger = Instantiate(objectPrefab, this.gameObject.transform.position, Quaternion.identity).GetComponent<DialogueTrigger>();
                dlTrigger.enabled = true;
                dlTrigger.SetKnotName(knotName);
            }
            else if (eObjDialogue == EObjDialogue.Npc)
            {
                NpcController npcGO = Instantiate(objectPrefab, this.gameObject.transform.position, Quaternion.identity).GetComponent<NpcController>();
                DialogueTrigger dialogueTrigger = npcGO.GetComponentInChildren<DialogueTrigger>();
                if (dialogueTrigger != null)
                {
                    dialogueTrigger.enabled = true;
                    dialogueTrigger.SetKnotName(knotName);
                }

                if (action == ENpcAction.Chopping)
                {
                    npcGO.StartChoppingTrees();
                }
            }
        }    
    }    
}
