using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoneAndMineral : ItemDropableEntity
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _onHitTime;
    private Coroutine _hitCoroutine;
    [SerializeField] private GameEvent _onMineralsDestroy;
    public enum StoneAndMineralType
    {
        Small,
        Big
    }

    public StoneAndMineralType stoneAndMineralType;
    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    [ClientRpc]
    public void InitializeMineralClientRpc(string entityId)
    {
        var entityInfo = ItemDropableEntityDatabase.Instance.GetEntity(entityId);
        if (entityInfo == null)
        {

            Debug.LogError("Entity not found in database: " + entityId);
            return;
        }
        entityInfo = ItemDropableEntityDatabase.Instance.GetEntity(entityId);
        _spriteRenderer.sprite = entityInfo.mineBlockIdleSprite;
    }

    public override void OnHit(Vector2 knockback)
    {
        if (!damageable.IsAlive)
        {
            //DropItemServerRpc(false);
            _onMineralsDestroy.Raise(this,null);
        }
        else
        {
            ChangeSpriteOnHit();
        }
    }
    private void ChangeSpriteOnHit()
    {
        if (_hitCoroutine != null)
        {
            StopCoroutine(_hitCoroutine);  
        }
        Debug.Log("run change sprite");
        _hitCoroutine = StartCoroutine(ChangeSpriteRoutine());
    }
    private IEnumerator ChangeSpriteRoutine()
    {
        _spriteRenderer.sprite = entityInfo.mineBlockHitSprite;
        yield return new WaitForSeconds(_onHitTime);
        _spriteRenderer.sprite = entityInfo.mineBlockIdleSprite;
        _hitCoroutine = null;  
    }

}
