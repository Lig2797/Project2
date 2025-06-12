using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class NpcController : MonoBehaviour
{
    public Transform targetPosition;
    public DetectionZone zone;
    private NavMeshAgent agent;

    [SerializeField] private Animator animator;
    [SerializeField] private PlayableDirector npcDirector;

    private Coroutine choppingRoutine;
    public float detectionRadius = 10f;
    public float raycastSpacing = 2f;
    public LayerMask treeLayer;

    private bool canAttack = true;
    private bool canMove = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (zone != null && zone.detectedColliders.Count > 0 && canAttack)
        {
            animator.Play("Axe_Attack");
        }

        if (targetPosition != null && canMove)
        {
            OnMove();
        }
        if (!canMove)
        {
            OnStop();
        }    
    }

    private void OnDestroy()
    {
        Debug.Log("NpcController destroyed.");
    }

    public void OnMove()
    {
        if (agent != null && agent.isActiveAndEnabled && targetPosition != null)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPosition.position);
            UpdateAnimation();
        }
    }

    public void OnStop()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

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

    public void SetTargetPosition(Transform newTarget)
    {
        targetPosition = newTarget;
    }

    public void StartChoppingTrees()
    {
        if (!canAttack || targetPosition == null || agent == null) return;

        agent.SetDestination(targetPosition.position);

        if (zone != null && zone.detectedColliders.Count > 0)
        {
            animator.Play("Axe_Attack");
        }
    }

    public void StopChoppingTrees()
    {
        if (choppingRoutine != null)
        {
            StopCoroutine(choppingRoutine);
            choppingRoutine = null;
        }

        if (agent != null)
        {
            agent.ResetPath();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector2 origin = transform.position;

        for (float angle = 0; angle < 360; angle += raycastSpacing)
        {
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Gizmos.color = Color.green;
            Gizmos.DrawRay(origin, dir * detectionRadius);

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, detectionRadius, treeLayer);

            if (hit.collider != null && hit.collider.CompareTag("Tree"))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(origin, dir * detectionRadius);

                if (targetPosition == null)
                {
                    targetPosition = hit.collider.transform;
                }
            }
        }
    }

    public void StopAllAction()
    {
        canAttack = false;
        canMove = false;
        StopChoppingTrees();
    }

    public void StartAllAction()
    {
        canAttack = true;
        canMove = true;
    }
}
