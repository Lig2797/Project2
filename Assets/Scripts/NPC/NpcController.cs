using System.Collections;
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

    private Coroutine choppingRoutine;
    public float detectionRadius = 5f;
    public float raycastSpacing = 45f;
    public LayerMask treeLayer;

    private void OnEnable()
    {
        //GameEventsManager.Instance.npcEvents.onNpcSpawned += PlayDirector;
        //GameEventsManager.Instance.npcEvents.onCallNpcHome += CallNpcHome;
    }

    private void OnDisable()
    {
        //GameEventsManager.Instance.npcEvents.onNpcSpawned -= PlayDirector;
        //GameEventsManager.Instance.npcEvents.onCallNpcHome -= CallNpcHome;
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

    public void StartChoppingTrees()
    {
        if (choppingRoutine == null)
            choppingRoutine = StartCoroutine(ChopTreeRoutine());
    }

    public void StopChoppingTrees()
    {
        if (choppingRoutine != null)
        {
            StopCoroutine(choppingRoutine);
            choppingRoutine = null;
        }

        agent.ResetPath();
        animator.SetFloat("Speed", 0f);
    }

    private IEnumerator ChopTreeRoutine()
    {
        while (true)
        {
            Transform tree = FindNearestTreeByRaycast2D();

            if (tree != null)
            {
                agent.SetDestination(tree.position);

                while (Vector2.Distance(transform.position, tree.position) > 0.5f)
                {
                    yield return null;
                }

                animator.SetFloat("Speed", 0f);
                Debug.Log("NPC is chopping tree: " + tree.name);
                yield return new WaitForSeconds(3f); // thời gian chặt cây
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }
        }
    }

    private Transform FindNearestTreeByRaycast2D()
    {
        for (float angle = 0; angle < 360; angle += raycastSpacing)
        {
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector2 origin = transform.position;

            // Vẽ ray trong Scene view
            Debug.DrawRay(origin, dir * detectionRadius, Color.green, 0.1f);

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, detectionRadius, treeLayer);

            if (hit.collider != null && hit.collider.CompareTag("Tree"))
            {
                // Đổi màu ray nếu trúng cây
                Debug.DrawRay(origin, dir * detectionRadius, Color.red, 0.5f);
                return hit.collider.transform;
            }
        }

        return null;
    }
}
