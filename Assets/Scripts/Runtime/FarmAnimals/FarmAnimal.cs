using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
public abstract class FarmAnimal : NetworkBehaviour
{
    #region Components
    [SerializeField] protected Rigidbody2D _body;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected FarmAnimalSO _animalInfo;
    #endregion

    #region Variables

    [SerializeField] protected string id;
    [ContextMenu("Generate guid for id")]
    protected void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] protected Gender gender;
    [SerializeField] protected bool canMakeProduct;

    protected int fedTimeCounter = 0;
    protected bool isFed = false;

    protected float maxRadius = 5f;
    protected float speed = 2f;
    protected Vector2 targetPosition;

    [SerializeField] protected bool isMoving = false;

    public NetworkVariable<bool> CanMove = new(true);

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

            if (_lastMovement.x > 0 && !_isFacingRight)
                IsFacingRight = true;
            else if (_lastMovement.x < 0 && _isFacingRight)
                IsFacingRight = false;
        }
    }

    private bool _isFacingRight = true;
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

    protected IEnumerator Start()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.IsListening);
        Debug.Log("run spawn");
        GetComponent<NetworkObject>().Spawn();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("animal spawned");
        CanMove.OnValueChanged += onValueChange;

        Initial();
        if (!IsServer) return;

        FarmAnimalManager.Instance.RegisterAnimal(this);
        CanMove.Value = true;
    }

    public override void OnNetworkDespawn()
    {
        CanMove.OnValueChanged -= onValueChange;

        if (!IsServer) return;

        FarmAnimalManager.Instance.UnregisterAnimal(this);
    }

    private void onValueChange(bool previousValue, bool newValue)
    {
        if (!newValue)
        {
            _body.linearVelocity = Vector2.zero;
        }
    }

    protected void Update()
    {
        if (!IsServer || !IsSpawned) return;

        if (!CanMove.Value)
        {
            _body.linearVelocity = Vector2.zero;
            return;
        }

        if (!isMoving)
        {
            ChooseNewTarget();
        }
        else
        {
            MoveToTarget();
        }

        SetAnimator();
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
        if (!CanMove.Value) return;

        Vector2 currentPosition = _body.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        _body.linearVelocity = direction * speed;

        if (direction != Vector2.zero)
            LastMovement = direction;

        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            isMoving = false;
            StartStopMoving(5);
        }
    }

    private IEnumerator StopMoving(int seconds)
    {
        // Wait until the object is properly spawned
        while (!IsSpawned)
        {
            yield return null;
        }

        if (IsServer)
        {
            CanMove.Value = false;
            _body.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(seconds);

        if (IsServer)
        {
            CanMove.Value = true;
            ChooseNewTarget(); // Optional: immediately choose new spot
        }
    }

    private void StartStopMoving(int seconds)
    {
        if (!IsServer || !IsSpawned) return;

        if (stopMovingCoroutine != null)
        {
            StopCoroutine(stopMovingCoroutine);
        }

        stopMovingCoroutine = StartCoroutine(StopMoving(seconds));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer || !IsSpawned) return;

        if (collision.gameObject.name == "Collision")
        {
            isMoving = false;
            StartStopMoving(5);
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
        isFed = true;
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
