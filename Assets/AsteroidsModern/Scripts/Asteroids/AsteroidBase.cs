using AsteroidsModern.Core;
using AsteroidsModern.Enums;
using AsteroidsModern.Interfaces;
using AsteroidsModern.Scripts.Effects;
using UnityEngine;

namespace AsteroidsModern.Scripts.Asteroids
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public abstract class AsteroidBase : MonoBehaviour, IAsteroid
    {
        private int _currentHealth;
        private int _maxHealth;
        private float _speed;
        private bool _isInitialized;

        protected AsteroidSpawner spawner;
        protected Rigidbody2D rigid;
        
        public bool IsDestroyed => _currentHealth <= 0;
        public AsteroidSize Size { get; private set; } = AsteroidSize.Small;

        public int ScoreValue { get; private set; }

        public Vector3 Position => transform.position;
        
        protected virtual void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            
            rigid.gravityScale = 0f;
            rigid.linearDamping = 0f;
            rigid.angularDamping = 0f;

            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        protected virtual void Update()
        {
            if (_isInitialized)
            {
                UpdateRotation();
            }
        }

        public virtual void Initialize(AsteroidSpawner asteroidSpawner, AsteroidSize asteroidSize, Vector2 direction)
        {
            spawner = asteroidSpawner;
            Size = asteroidSize;
            _speed = spawner.GetSpeedForSize(Size);
            _maxHealth = spawner.GetHealthForSize(Size);
            ScoreValue = spawner.GetScoreForSize(Size);
            _currentHealth = _maxHealth;
            ConfigureForSize();
            Move(direction, _speed);
            _isInitialized = true;
            
            GameEvents.TriggerAsteroidSpawned(this);
        }

        protected abstract void ConfigureForSize();

        private void Move(Vector2 direction, float moveSpeed)
        {
            if (rigid != null)
            {
                rigid.linearVelocity = direction.normalized * moveSpeed;
            }
        }

        private void OnGamePaused()
        {
            if (rigid != null)
            {
                rigid.simulated = false;
            }
            _isInitialized = false;
        }
        
        private void OnGameResumed()
        {
            if (rigid != null)
            {
                rigid.simulated = true;
            }
            _isInitialized = true;
        }

        public virtual void TakeDamage(int damage)
        {
            if (IsDestroyed) return;

            _currentHealth = Mathf.Max(0, _currentHealth - damage);

            if (_currentHealth <= 0)
            {
                DestroyAsteroid();
            }
        }

        public void InstantDestroy()
        {
            if (IsDestroyed) return;

            _currentHealth = 0;
            Destroy(gameObject);
        }

        protected virtual void DestroyAsteroid()
        {
            GameEvents.OnGamePaused -= OnGamePaused;
            GameEvents.OnGameResumed -= OnGameResumed;
            
            EffectsManager.Instance.ShakeScreen(0.1f, 0.2f);
            
            GameEvents.TriggerAsteroidDestroyed(this);
            Destroy(gameObject);
        }

        private void UpdateRotation()
        {
            rigid.rotation += 50f * Time.deltaTime;
        }
    }
}