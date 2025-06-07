using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class NpcController : MonoBehaviour
{
    public Transform targetPosition;
    private NavMeshAgent agent;

    [SerializeField] private Animator animator;
    [SerializeField] private PlayableDirector npcDirector;

    private void OnEnable()
    {
        //GameEventsManager.Instance.npcEvents.onNpcSpawned += PlayDirector;
        GameEventsManager.Instance.npcEvents.onCallNpcHome += CallNpcHome;
    }

    private void OnDisable()
    {
        //GameEventsManager.Instance.npcEvents.onNpcSpawned -= PlayDirector;
        GameEventsManager.Instance.npcEvents.onCallNpcHome -= CallNpcHome;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            UpdateAnimation();
        }
    }

    private void OnDestroy()
    {
        Debug.Log("AIController destroyed, stopping PlayableDirector if playing.");
    }

    //private void PlayDirector()
    //{
    //    if (npcDirector != null)
    //    {
    //        npcDirector.Play();
    //    }
    //    else
    //    {
    //        Debug.LogWarning("PlayableDirector is not assigned.");
    //    }
    //}    

    private void UpdateAnimation()
    {
        float speed = agent.velocity.magnitude;

        if (speed > 0.1f)
        {
            Vector3 movementDirection = agent.velocity.normalized;

            animator.SetFloat("Horizontal", movementDirection.x);
            animator.SetFloat("Vertical", movementDirection.z);
            animator.SetFloat("Speed", speed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    private void CallNpcHome()
    {
        if (targetPosition != null)
        {
            agent.SetDestination(targetPosition.position);
        }
    }

    public void SetTargetPosition(Transform newTarget)
    {
        targetPosition = newTarget;
    }
}
