using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
public abstract class FarmAnimal : MonoBehaviour
{
    #region Components
    [SerializeField] protected Rigidbody2D _body;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected FarmAnimalSO _animalInfo;
    #endregion

    #region Variables

    [SerializeField] protected string id;
    [ContextMenu("Generate guid for id")]
    protected void GenerateGuid() => id = System.Guid.NewGuid().ToString();

    [SerializeField] protected Gender gender;
    [SerializeField] protected bool canMakeProduct = false;

    [SerializeField]
    protected int fedTimeCounter = 0;
    [SerializeField]
    protected int resetFedTime = 1000;
    [SerializeField]
    protected bool isFed = false;

    protected float maxRadius = 5f;
    protected float speed = 2f;
    protected Vector2 targetPosition;

    [SerializeField] protected bool isMoving = false;

    [SerializeField] protected bool _canMove = true;
    public bool CanMove
    {
        get => _canMove;
        set
        {
            _canMove = value;
            if (!_canMove)
                _body.linearVelocity = Vector2.zero;
        }
    }

    [Header("Obstacle Detection")]
    [SerializeField] protected LayerMask collisionMask;
    [SerializeField] private float rayDistance = 1.5f;
    [SerializeField] private float rayAngleSpread = 30f; // total cone angle

    #endregion

    #region Animation variables
    protected Vector2 _lastMovement;
    public Vector2 LastMovement
    {
        get => _lastMovement;
        set
        {
            _lastMovement = value;
            float clampedX = Mathf.Abs(Mathf.Round(_lastMovement.x));
            float clampedY = Mathf.Round(_lastMovement.y);
            _animator.SetFloat(HorizontalParameter, clampedX);
            _animator.SetFloat(VerticalParameter, clampedY);

            if (_lastMovement.x > 0 && !_isFacingRight) IsFacingRight = true;
            else if (_lastMovement.x < 0 && _isFacingRight) IsFacingRight = false;
        }
    }

    private bool _isFacingRight = false;
    public bool IsFacingRight
    {
        get => _isFacingRight;
        set
        {
            _isFacingRight = value;
            transform.localScale = _isFacingRight ? new Vector3(-1, 1, 1) : Vector3.one;
        }
    }

    public string HorizontalParameter => "Horizontal";
    public string VerticalParameter => "Vertical";
    #endregion

    private Coroutine stopMovingCoroutine;

    protected void OnEnable()
    {
        FarmAnimalManager.Instance.RegisterAnimal(this);
    }

    protected void OnDisable()
    {
        FarmAnimalManager.Instance.UnregisterAnimal(this);
    }
    private void Start()
    {

        Initial();
    }
    protected void Update()
    {
        SetAnimator();
        if (!CanMove) return;

        if (!isMoving)
            ChooseNewTarget();
        else
            MoveToTarget();
    }

    private void ChooseNewTarget()
    {
        Vector2 currentPosition = transform.position;
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);
        float randomDistance = Random.Range(0f, maxRadius);
        Vector2 offset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomDistance;
        targetPosition = currentPosition + offset;
        isMoving = true;
    }

    private void MoveToTarget()
    {
        if (_body.linearVelocity.sqrMagnitude > 0.01f && CheckFanObstacle())
        {
            isMoving = false;
            StartStopMoving(5);
            return;
        }

        Vector2 currentPosition = _body.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        _body.linearVelocity = direction * speed;

        if (direction != Vector2.zero)
            LastMovement = direction;

        if (Vector2.Distance(currentPosition, targetPosition) < 0.5f)
        {
            isMoving = false;
            StartStopMoving(5);
        }

        Debug.DrawRay(currentPosition, LastMovement * rayDistance, Color.yellow);
    }


    private bool CheckFanObstacle()
    {
        if (LastMovement == Vector2.zero) return false;

        Vector2 origin = _body.position;
        Vector2 forward = LastMovement.normalized;
        float half = rayAngleSpread * 0.5f;
        float[] angles = { -half, 0f, half };

        foreach (float a in angles)
        {
            Vector2 dir = Quaternion.Euler(0, 0, a) * forward;
            Debug.DrawRay(origin, dir * rayDistance, Color.red);
            if (Physics2D.Raycast(origin, dir, rayDistance, collisionMask))
                return true;
        }
        return false;
    }

    private IEnumerator StopMoving(int seconds)
    {
        CanMove = false;
        yield return new WaitForSeconds(seconds);
        CanMove = true;
    }

    

    private void StartStopMoving(int seconds = 5)
    {
        if (stopMovingCoroutine != null)
            StopCoroutine(stopMovingCoroutine);
        stopMovingCoroutine = StartCoroutine(StopMoving(seconds));
    }

    // Draw the fan cone in the Scene view when selected
    private void OnDrawGizmosSelected()
    {
        if (_body == null) return;
        Vector2 origin = Application.isPlaying ? _body.position : (Vector2)transform.position;
        Vector2 forward = LastMovement.normalized;
        if (forward == Vector2.zero)
            forward = Vector2.right;

        Gizmos.color = Color.red;
        float half = rayAngleSpread * 0.5f;
        float[] angles = { -half, 0f, half };
        foreach (float a in angles)
        {
            Vector2 dir = Quaternion.Euler(0, 0, a) * forward;
            Gizmos.DrawLine(origin, origin + dir * rayDistance);
        }
    }

    public void SetAnimator()
    {
        _animator.SetFloat("Speed", _body.linearVelocity.magnitude);
    }

    [ContextMenu("Eat")]
    protected virtual void Eat()
    {
        if (isFed) return;
        _animator.SetTrigger("Eat");
        StartStopMoving(5);
        isFed = true;
    }

    protected void ChangeResetFedTime(int value = 1000)
    {
        resetFedTime = value;
    }
    public abstract void FedTimeHandler(int minute);
    protected virtual void MakeProduct() { }
    protected virtual void GetProduct() { }
    protected virtual void InteractWithAnimal() { }
    public abstract void IncreaseGrowStage();

    protected virtual void ApplyStage(string stage)
    {
        _animator.SetTrigger(stage);
    }

    protected virtual void Initial()
    {
        gender = _animalInfo.Gender;
        GenerateGuid();
    }
}
