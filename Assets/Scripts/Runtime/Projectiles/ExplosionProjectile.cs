using UnityEngine;

public class ExplosionProjectile : MonoBehaviour
{
    public LayerMask hitAbleLayer;
    [SerializeField] private float _knockbackStrength;
    [HideInInspector] public float DamageAmount; // set by the person shoot it
    [SerializeField] private GameObject _shadowObject;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitAbleLayer) == 0)
            return;

        Damageable damageable = collision.gameObject.GetComponent<Damageable>();
        if (damageable != null)
        {
            GameObject parentObject = transform.root.gameObject;
            Vector3 explosionPosition = parentObject.transform.position;

            Vector2 direction = (collision.transform.position - explosionPosition).normalized;
            Vector2 deliveredKnockBack = direction * _knockbackStrength; // Example knockback strength

            damageable.Hit(DamageAmount, deliveredKnockBack); // Example damage value

        }
    }

    public void DestroyAfterDoneExplosion()
    {
        Destroy(transform.root.gameObject);
        
    }

    public void DestroyShadowObject()
    {
        AudioManager.Instance.PlaySFX("explosion");
        Destroy(_shadowObject);
    }

}
