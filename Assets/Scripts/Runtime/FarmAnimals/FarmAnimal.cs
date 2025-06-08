using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

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

    protected FarmAnimalSO farmAnimalSO;

    [SerializeField] protected Gender gender;
    [SerializeField] protected bool canMakeProduct;

    protected int fedTimeCounter = 0;
    protected bool isFed = false;

    protected float maxRadius = 5f; // Maximum movement radius
    protected float speed = 2f; // Movement speed
    protected Vector2 targetPosition;

    [SerializeField] protected bool isMoving = false;

    public NetworkVariable<bool> CanMove = new(true);


    #endregion

    #region Animation variables
    protected Vector2 _lastMovement;
    public Vector2 LastMovement
    {
        get { return _lastMovement; }
        set
        {
            _lastMovement = value;
            // Clamp or round to -1, 0, or 1 to snap to a single direction
            float clampedX = Mathf.Abs(Mathf.Round(_lastMovement.x));
            float clampedY = Mathf.Round(_lastMovement.y);

            _animator.SetFloat(HorizontalParameter, clampedX);
            _animator.SetFloat(VerticalParameter, clampedY);
            if (_lastMovement.x > 0 && !IsFacingRight)
            {
                IsFacingRight = true;
            }
            else if (_lastMovement.x < 0 && IsFacingRight)
            {
                IsFacingRight = false;
            }
        }

    }
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        set
        {
            _isFacingRight = value;
            transform.localScale = _isFacingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        }
    }
    public string HorizontalParameter
    {
        get => "Horizontal";
    }

    public string VerticalParameter
    {
        get => "Vertical";
    }
    #endregion

    private Coroutine stopMovingCoroutine;

    protected void Start()
    {
        Initial();
    }

    public override void OnNetworkSpawn()
    {
        CanMove.OnValueChanged += onValueChange;
        if (!IsServer) return;
        FarmAnimalManager.Instance.RegisterAnimal(this);
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
            _body.linearVelocity = Vector2.zero;
        Debug.Log("Did run on can move changed");
    }
    protected void Update()
    {
        if (CanMove.Value)
        {
            if (!isMoving)
            {
                ChooseNewTarget();
            }
            else
            {
                MoveToTarget();
            }
        }
        SetAnimator();
    }




    [ContextMenu("Eat")]
    protected virtual void Eat()
    {
        if(isFed) return;
        _animator.SetTrigger("Eat");
        isFed = true;

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

    private IEnumerator StopMoving(int minute)
    {
        CanMove.Value = false; 

        yield return new WaitForSeconds(minute); 

        CanMove.Value = true; 
    }

    private void StartStopMoving(int minute)
    {
        if (stopMovingCoroutine != null)
        {
            StopCoroutine(stopMovingCoroutine);
        }

        stopMovingCoroutine = StartCoroutine(StopMoving(minute));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Collision")
        {
            StartStopMoving(5);
        }
        
    }

    public void SetAnimator()
    {
        _animator.SetFloat("Speed", _body.linearVelocity.magnitude);

    }
    public abstract void FedTimeHandler(int minute);
    protected virtual void MakeProduct() { } // each animal will have different way to product
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
