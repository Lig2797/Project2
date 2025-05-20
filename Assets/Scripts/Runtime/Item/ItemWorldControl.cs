using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ItemWorldControl : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> id = new NetworkVariable<FixedString64Bytes>();
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id.Value = System.Guid.NewGuid().ToString();
    }
    public Item item;
    private ItemWorld _itemWorld;
    private Rigidbody2D rb;
    private Collider2D _collider2D;
    private Collider2D _TargetzoneCollider2D;
    private TargetZone _targetZone;
    public NetworkVariable<bool> CanPickup = new NetworkVariable<bool>(false);
    public Transform targetTransform;

    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxSpeed;

    private Vector3 _currentVelocity = Vector2.zero;

    // GameEvent
    [SerializeField] private GameEvent onItemWorldTouchPlayer;
    private void OnEnable()
    {
        CanPickup.OnValueChanged += UpdateCollider;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _targetZone = GetComponentInChildren<TargetZone>();
        _collider2D = GetComponent<Collider2D>();
        _TargetzoneCollider2D = GetComponentInChildren<Collider2D>();
    }

    public override void OnNetworkSpawn()
    {
        ItemWorldManager.Instance.FindItemInListAndInitializeAfterClientJoin(this, id.Value.ToString());
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        CanPickup.OnValueChanged -= UpdateCollider;
    }
    private void FixedUpdate()
    {
        if (targetTransform == null)
        {
            _currentVelocity = Vector2.zero;
            rb.velocity = Vector2.zero;

        }
        else
        {
            Vector3 currentPos = rb.position;
            Vector3 direction = (targetTransform.position - currentPos).normalized;

            _currentVelocity = direction * _acceleration;
            _currentVelocity = Vector2.ClampMagnitude(_currentVelocity, _maxSpeed);
            _acceleration += 0.1f;
            rb.velocity = _currentVelocity;
        }
        
    }

    public void SetTargetTransform(Transform playerTransform)
    {
        targetTransform = playerTransform;
    }

    public void SetItemImage(Sprite image)
    {
        transform.GetComponent<SpriteRenderer>().sprite = item.image;
    }

    public ItemWorld GetItemWorld()
    {
        return _itemWorld;
    }

    public void InitialItemWorld(ItemWorld itemWorld = null)
    {
        if(itemWorld == null)
        {
            itemWorld = new ItemWorld(System.Guid.NewGuid().ToString(), item, 1, transform.position);
        }
        _itemWorld = itemWorld;
        item = itemWorld.Item;
        SetItemImage(item.image);
        if(IsServer)
        StartCoroutine(DelayedSetId(itemWorld.Id));
    }
    private IEnumerator DelayedSetId(string value)
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.IsListening && IsSpawned);
        id.Value = value;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player") && CanPickup.Value)
        {
            
            var player = collision.GetComponent<PlayerController>();
            if (player == null) return;
            Debug.Log("Player touched item");
            _collider2D.enabled = false;    
            if(player.IsLocalPlayer)
            onItemWorldTouchPlayer.Raise(this, null);
        }
    }



    public void StartWaitForPickup(float timesTillCanPickup)
    {
        if(IsServer)
        StartCoroutine(WaitForPickup(timesTillCanPickup));
    }

    IEnumerator WaitForPickup(float timesTillCanPickup)
    {
        CanPickup.Value = false;

        yield return new WaitForSeconds(timesTillCanPickup);

        CanPickup.Value = true;
    }

    public void UpdateCollider(bool oldValue, bool newValue)
    {
        if(newValue)
        {
            _collider2D.enabled = true;
            _TargetzoneCollider2D.enabled = true;
        }
        else
        {
            _targetZone.SetTargetTransform(this.GetComponent<NetworkObject>()); // this mean null
            _collider2D.enabled = false;
            _TargetzoneCollider2D.enabled = false;
        }
    }
    

}


