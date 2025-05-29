using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>, IDataPersistence
{
    #region Setup Everything
    #region Components
    [Header("Components")]
    [SerializeField] private TileTargeter tileTargeter;
    [SerializeField] private InventoryController _inventoryController;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;
    #endregion

    #region PlayerStatus
    [Header("Player Status")]
    [SerializeField] private float _acceleration;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float _vehicleSpeed;

    public float VehicleSpeed
    {
        get { return _vehicleSpeed; }
        set { _vehicleSpeed = value; }
    }
    private string _currentState;
    public string CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }
    public string[] noTargetStates;
    public string[] toolsAndWeapon;



    [SerializeField]
    private bool _canMove = true;
    public bool CanMove
    {
        get { return _canMove; }
        set
        {
            _canMove = value;
        }
    }

    [SerializeField]
    private float _currentSpeed;
    public float CurrentSpeed
    {
        get
        {
            return _currentSpeed = CanMove ? IsRidingVehicle ? _vehicleSpeed : IsRunning ? runSpeed : walkSpeed : 0;
        }
    }
    [SerializeField]
    private Vector2 _movement;
    public Vector2 Movement
    {
        get { return _movement; }
        set { _movement = value; }
    }
    private Vector2 _lastMovement;
    public Vector2 LastMovement // Keep the last animation
    {
        get { return _lastMovement; }
        set
        {
            _lastMovement = value;
            animator.SetFloat("Horizontal", Mathf.Abs(_lastMovement.x));
            animator.SetFloat("Vertical", _lastMovement.y);
        }

    }
    [SerializeField]
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

    private bool _canRun = true;
    public bool CanRun
    {
        get { return _canRun; }
        private set { _canRun = value; }
    }

    private bool _isRuning = false;
    public bool IsRunning
    {
        get { return _isRuning; }
        private set
        {
            _isRuning = value;
            animator.SetBool("IsRunning", _isRuning);
        }
    }

    private bool _canRide = false;
    public bool CanRide
    {
        get { return _canRide; }
        private set { _canRide = value; }
    }


    private bool _isRidingVehicle = false;
    public bool IsRidingVehicle
    {
        get { return _isRidingVehicle; }
        private set
        {
            _isRidingVehicle = value;
            if (value)
            {
                if (CurrentVehicle.tag == "Bicycle")
                    animator.SetBool("UseDevice", true);

                else if (CurrentVehicle.tag == "Horse")
                    animator.SetBool("UseHorse", true);
            }
            else
            {
                animator.SetBool("UseDevice", false);
                animator.SetBool("UseHorse", false);
            }

        }
    }

    private bool _isHoldingItem = false;
    public bool IsHoldingItem
    {
        get { return _isHoldingItem; }
        private set
        {
            _isHoldingItem = value;
        }
    }

    private bool _canAttack = true;
    public bool CanAttack
    {
        get { return _canAttack; }
        private set { _canAttack = value; }
    }

    private bool _canSleep = false;
    public bool CanSleep
    {
        get { return _canSleep; }
        private set { _canSleep = value; }
    }

    private bool _isSleeping = false;
    public bool IsSleeping
    {
        get { return _isSleeping; }
        private set { _isSleeping = value; }
    }

    private bool _hadTarget;
    public bool HadTarget
    {
        get { return _hadTarget; }
        private set { _hadTarget = value; }
    }
    #endregion

    #region Dependencies Scripts
    [Header("Dependencies")]
    [SerializeField] private InputReader _inputReader;
    private Player player;

    private VehicleController _currentVehicle;
    public VehicleController CurrentVehicle
    {
        get { return _currentVehicle; }
        private set
        {
            _currentVehicle = value;
            if (_currentVehicle == null)
            {
                HadTarget = false;
                CanRide = false;
            }
            else
            {
                HadTarget = true;
                CanRide = true;
            }
        }
    }



    private BedScript _currentBed;
    public BedScript CurrentBed
    {
        get { return _currentBed; }
        private set
        {
            _currentBed = value;
            if (_currentBed == null)
            {
                HadTarget = false;
                CanSleep = false;
            }
            else
            {
                HadTarget = true;
                CanSleep = true;
            }
        }
    }



    [SerializeField]
    private ItemOnHand _itemOnHand;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;


    [SerializeField] private InventoryManagerSO _inventoryManagerSO;
    #endregion

    #region Game Events
    [SerializeField] private GameEvent onPlayerLoadEvent;
    [SerializeField] private GameEvent onPlayerSaveEvent;
    [SerializeField] private GameEvent onSubmit;
    #endregion
    #endregion


    #region Setup Before Game Start
    protected override void Awake()
    {
        base.Awake();

        _inventoryController = GetComponent<InventoryController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        _inputReader.playerActions.moveEvent += OnMove;
        _inputReader.playerActions.attackEvent += OnAttack;
        _inputReader.playerActions.interactEvent += OnInteract;
        _inputReader.playerActions.secondInteractEvent += OnSecondInteract;
        _inputReader.playerActions.runEvent += OnRun;
        _inputReader.playerActions.submitEvent += OnSubmit;
        _inventoryManagerSO.onChangedSelectedSlot += CheckAnimation;
    }

    private void OnDisable()
    {
        _inputReader.playerActions.moveEvent -= OnMove;
        _inputReader.playerActions.attackEvent -= OnAttack;
        _inputReader.playerActions.interactEvent -= OnInteract;
        _inputReader.playerActions.secondInteractEvent -= OnSecondInteract;
        _inputReader.playerActions.runEvent -= OnRun;
        _inputReader.playerActions.submitEvent -= OnSubmit;
        _inventoryManagerSO.onChangedSelectedSlot -= CheckAnimation;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            DontDestroyOnLoad(gameObject);

            bool isHost = NetworkManager.Singleton.IsHost && IsServer; // true only on host machine
        }
    }

    
    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            bool isHost = NetworkManager.Singleton.IsHost && IsServer; // true only on host machine
            DataPersistenceManager.Instance.SaveGame();
        }
    }

    private void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
        else if (IsOwner && SceneManagement.GetCurrentSceneName().Equals(Loader.Scene.WorldScene.ToString()))
        {
            virtualCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
            virtualCamera.Follow = transform;
        }
    }

    #endregion

    #region Game Loop
    //void Update()
    //{
    //    CheckAnimation();
    //}

    private void FixedUpdate()
    {
        MovementHandler();
    }

    public void EnableControl()
    {         
        if (_inputReader != null) _inputReader.EnableControl();
    }
    
    public void DisableControl()
    {
        if (_inputReader != null) _inputReader.DisableControl();
    }

    #endregion

    #region Bed Setup
    public void SetCurrentBed(BedScript bed)
    {
        if (HadTarget) return;
        CurrentBed = bed;
        CurrentBed.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void ClearBed()
    {
        CurrentBed.GetComponent<SpriteRenderer>().color = Color.white;
        CurrentBed = null;

    }
    #endregion

    #region Vehicle Setup
    public void SetCurrentVehicle(VehicleController vehicle)
    {
        if (HadTarget) return;
        CurrentVehicle = vehicle;
        CurrentVehicle.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void ClearVehicle()
    {
        CurrentVehicle.GetComponent<SpriteRenderer>().color = Color.white;
        CurrentVehicle = null;
    }

    [ServerRpc]
    private void RequestToRideVehicleServerRpc(NetworkObjectReference playerRef, NetworkObjectReference vehicleRef, ServerRpcParams rpcParams = default)
    {
        if (playerRef.TryGet(out NetworkObject playerObj) && vehicleRef.TryGet(out NetworkObject vehicleObj))
        {
            var player = playerObj.GetComponent<PlayerController>();
            var vehicle = vehicleObj.GetComponent<VehicleController>();

            vehicle.SetRiding(true, playerRef);
            vehicle.transform.SetParent(playerObj.transform);

            FixVehicleLocalScaleClientRpc(vehicleRef, playerRef);
        }
    }


    [ClientRpc]
    private void FixVehicleLocalScaleClientRpc(NetworkObjectReference vehicleRef, NetworkObjectReference playerRef)
    {
        if (vehicleRef.TryGet(out NetworkObject vehicleObj) && playerRef.TryGet(out NetworkObject playerObj))
        {
            var player = playerObj.GetComponent<PlayerController>();
            var vehicle = vehicleObj.GetComponent<VehicleController>();
            if (vehicle.transform.localScale.x < 0) vehicle.transform.localScale = new Vector3(1, 1, 1);
            player.IsFacingRight = vehicle.IsFacingRight.Value;
        }

    }

    [ServerRpc]
    private void RequestToUnRideVehicleServerRpc(NetworkObjectReference vehicleRef)
    {
        if (vehicleRef.TryGet(out NetworkObject vehicleObj))
        {
            vehicleObj.transform.SetParent(null, true);
        }
        RequestToUnRideVehicleClientRpc(vehicleRef);
    }

    [ClientRpc]
    private void RequestToUnRideVehicleClientRpc(NetworkObjectReference vehicleRef)
    {
        if (vehicleRef.TryGet(out NetworkObject vehicleObj))
            vehicleObj.GetComponent<VehicleController>().SetRiding(false, GetComponent<NetworkObject>());
    }
    #endregion

    #region Movement

    private void MovementHandler()
    {
        if (!CanMove)
        {
            if (rb.linearVelocity.magnitude > 0.1f)
                rb.AddForce(rb.linearVelocity * -_acceleration, ForceMode2D.Force);
            else
                rb.linearVelocity = Vector2.zero;

            return;
        }
        if (_movement != Vector2.zero)
        {
            rb.AddForce(_movement * _acceleration, ForceMode2D.Force);
            if (!IsRidingVehicle)
            {
                if (!IsRunning)
                {
                    if (rb.linearVelocity.magnitude > walkSpeed)
                    {
                        rb.linearVelocity = rb.linearVelocity.normalized * walkSpeed;
                    }
                }
                else
                {
                    if (rb.linearVelocity.magnitude > runSpeed)
                    {
                        rb.linearVelocity = rb.linearVelocity.normalized * runSpeed;
                    }
                }
            }
            else
            {
                if (rb.linearVelocity.magnitude > _vehicleSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * _vehicleSpeed;
                }
            }
        }
        else // do deceleration
        {
            if (rb.linearVelocity.magnitude > 0.1f)
                rb.AddForce(rb.linearVelocity * -_acceleration, ForceMode2D.Force);
            else
                rb.linearVelocity = Vector2.zero;
        }
        //rb.MovePosition(rb.position + _movement * CurrentSpeed * Time.fixedDeltaTime);
    }
    public void OnMove(Vector2 inputMovement)
    {
        _movement = inputMovement.normalized;

        if (_movement != Vector2.zero && CanMove)
        {
            LastMovement = _movement;

            if (_movement.x > 0 && !IsFacingRight) IsFacingRight = true;
            else if (_movement.x < 0 && IsFacingRight) IsFacingRight = false;
        }

        animator.SetFloat("Speed", _movement.magnitude);

        if (IsRidingVehicle)
        {
            SetCurrentVehicleMovementServerRpc(CurrentVehicle.GetComponent<NetworkObject>(), _movement, IsFacingRight);
        }

    }


    [ServerRpc]
    private void SetCurrentVehicleMovementServerRpc(NetworkObjectReference vehicleRef, Vector2 movement, bool IsFacingRight)
    {
        if (vehicleRef.TryGet(out NetworkObject vehicleObj))
        {
            var vehicle = vehicleObj.GetComponent<VehicleController>();
            vehicle.SetMovement(movement);
            if (movement != Vector2.zero)
                vehicle.IsFacingRight.Value = IsFacingRight;
        }
    }
    #endregion

    #region Actions Block
    public void StopAllAction()
    {
        CanMove = false;
        CanAttack = false;
    }

    public void StartAllAction()
    {
        CanMove = true;
        CanAttack = true;
        OnMove(_movement);
        CheckAnimation();
    }
    #endregion

    #region Animation
    public void CheckAnimation()
    {
        if (!CanAttack || IsRidingVehicle) return;
        Item item = _inventoryManagerSO.GetCurrentItem();
        _itemOnHand.ActivateItemOnHandServerRpc(null, false);
        if (item != null)
        {
            IsHoldingItem = true;
        }
        else
        {
            IsHoldingItem = false;
        }

        if (IsHoldingItem)
        {
            switch (item.type)
            {
                default:
                    {
                        ChangeAnimationState("Idle");

                        break;
                    }
                case ItemType.Tool:
                    {

                        ChangeAnimationState(item.name);

                        break;
                    }
                case ItemType.Crop:
                case ItemType.Food:
                    {
                        _itemOnHand.ActivateItemOnHandServerRpc(item.itemName, true);
                        ChangeAnimationState("Pickup_idle");
                        break;
                    }
            }
        }
        else ChangeAnimationState(AnimationStrings.idle);
    }


    private void ChangeAnimationState(string newState)
    {
        if (CurrentState == newState) return;

        animator.Play(newState);
        CurrentState = newState;
        tileTargeter.RefreshTilemapCheck(!noTargetStates.Contains(newState));

    }
    #endregion

    #region Actions
    private void OnAttack()
    {
        if (!IsRidingVehicle && IsHoldingItem && CanAttack && Input.GetMouseButton(0) && !_inventoryManagerSO.IsPointerOverUI)
        {
            UseCurrentItem();
        }
    }

    private void UseCurrentItem()
    {
        Item item = _inventoryManagerSO.GetCurrentItem();
        if(item == null) return;
        switch (item.type)
        {
            default:
                {
                    break;
                }
            case ItemType.Tool:
                {
                    animator.SetTrigger("Attack");
                    tileTargeter.UseTool(!noTargetStates.Contains(CurrentState));
                    break;
                }
            case ItemType.Crop:
                {
                    tileTargeter.SetTile(item);
                    break;
                }
        }
    }
    private void OnInteract()
    {
        if (!IsRidingVehicle && CanAttack && Input.GetMouseButton(1))
        {
            if (tileTargeter.CheckHarverst())
            {
                animator.SetTrigger(AnimationStrings.pickup);
                _itemOnHand.gameObject.SetActive(false);
                CurrentState = null;
            }
        }
    }

    private void OnSubmit()
    {
        onSubmit.Raise(this, ActionMap.Player);
        _inputReader.SwitchActionMap(ActionMap.UI);
    }

    // Load & Save
    private void OnSecondInteract()
    {
        if (CanRide && CurrentVehicle != null)
        {
            IsRidingVehicle = !IsRidingVehicle;
            if (IsRidingVehicle)
            {
                ChangeAnimationState("Idle");
                LastMovement = CurrentVehicle.VehicleLastMovement.Value;
                StartAllAction();
                RequestToRideVehicleServerRpc(
                    GetComponent<NetworkObject>(),
                    CurrentVehicle.GetComponent<NetworkObject>()
                );
            }
            else
            {
                RequestToUnRideVehicleServerRpc(
                    CurrentVehicle.GetComponent<NetworkObject>()
                );
            }
        }

        if (CanSleep)
        {
            if (IsSleeping)
            {
                StartAllAction();
                IsSleeping = !IsSleeping;
                CurrentBed.SetSleep(IsSleeping);
                animator.SetBool(AnimationStrings.isSleep, false);
            }
            else
            {
                StopAllAction();
                animator.SetBool(AnimationStrings.isSleep, true);

                IsSleeping = !IsSleeping;
                CurrentBed.SetSleep(IsSleeping);
            }
        }
    }
    private void OnRun(InputAction.CallbackContext context)
    {
        if (!CanRun) return;
        if (context.phase == InputActionPhase.Started) IsRunning = true;
        else if (context.phase == InputActionPhase.Canceled) IsRunning = false;
    }
    #endregion

    #region Save and Load

    public void LoadData(GameData gameData)
    {
        player = gameData.PlayerData;
        this.gameObject.transform.position = player.Position;
    }

    public void SaveData(ref GameData gameData)
    {
        player.SetPosition(transform.position);
        gameData.SetPlayerData(player);
    }
    #endregion
}