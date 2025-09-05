using AsteroidsModern.Core;
using AsteroidsModern.Interfaces;
using UnityEngine;

namespace AsteroidsModern.Weapons
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour, IProjectile
    {
        [SerializeField] private LayerMask targetLayers = -1;
        [SerializeField] private Rigidbody2D rigid;
        
        private int _damage = 1;
        private float _lifetimeTimer;
        private bool _isInitialized;
        
        private void Awake()
        {
            rigid.gravityScale = 0f;
            rigid.linearDamping = 0f;
        }

        public void Initialize(Vector2 direction, float speed, int dmg = 1, float lifetime = 3f)
        {
            _damage = dmg;
            _lifetimeTimer = lifetime;
            _isInitialized = true;
            
            rigid.linearVelocity = direction.normalized * speed;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

        private void Update()
        {
            if (!_isInitialized) return;

            UpdateLifetime();
        }

        private void UpdateLifetime()
        {
            _lifetimeTimer -= Time.deltaTime;
            if (_lifetimeTimer <= 0f)
            {
                DestroyProjectile();
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isInitialized) return;

            if (((1 << other.gameObject.layer) & targetLayers) == 0) return;

            var contact = other.ClosestPoint(transform.position);
            GameEvents.TriggerAsteroidHit(contact);
            
            var damageable = other.GetComponent<IDamageable>();
            if (damageable is { IsDestroyed: false })
            {
                OnHit(damageable);
            }
        }

        private void OnHit(IDamageable target)
        {
            if (target == null || target.IsDestroyed) return;

            target.TakeDamage(_damage);
            DestroyProjectile();
        }

        private void DestroyProjectile()
        {
            // Visual effect 
            //Destroy(gameObject);
            PoolManager.Instance.ReturnToPool(gameObject);
        }
    }
}