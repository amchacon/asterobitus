using AsteroidsModern.Core;
using AsteroidsModern.Interfaces;
using AsteroidsModern.Scripts.Effects;
using UnityEngine;

namespace AsteroidsModern.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem explosionFx;
        
        private int _maxHealth;
        private int _currentHealth;
        private float _invulnerabilityTime = 2f;
        private bool _isInvulnerable;
        private float _invulnerabilityTimer;

        private void Awake()
        {
            explosionFx.Stop();
        }

        public bool IsDestroyed => _currentHealth <= 0;

        internal void Initialize(GameSettings settings)
        {
            _maxHealth = settings.playerMaxHealth;
            _invulnerabilityTime = settings.playerInvulnerabilityTime;
            
            _currentHealth = _maxHealth;
            
            GameEvents.TriggerPlayerHealthChanged(_currentHealth);
        }
        
        private void Update()
        {
            UpdateInvulnerability();
        }

        public void TakeDamage(int damage)
        {
            if (_isInvulnerable || IsDestroyed) return;

            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            GameEvents.TriggerPlayerHealthChanged(_currentHealth);

            EffectsManager.Instance.ShakeScreen(0.3f, 0.2f);
            
            if (_currentHealth <= 0)
            {
                Die();
                return;
            }

            StartInvulnerability();
        }
        
        public void InstantDestroy()
        {
            if (IsDestroyed) return;

            _currentHealth = 0;
            GameEvents.TriggerPlayerHealthChanged(_currentHealth);
            Die();
        }

        private void Die()
        {
            GameEvents.TriggerPlayerDestroyed();
            explosionFx.Play(true);
        }

        //TODO: to be implemented - collectible to heal player
        public void Heal(int amount)
        {
            if (IsDestroyed) return;

            int previousHealth = _currentHealth;
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
            
            if (_currentHealth != previousHealth)
            {
                GameEvents.TriggerPlayerHealthChanged(_currentHealth);
            }
        }

        private void StartInvulnerability()
        {
            _isInvulnerable = true;
            _invulnerabilityTimer = _invulnerabilityTime;
        }

        private void UpdateInvulnerability()
        {
            if (!_isInvulnerable) return;

            _invulnerabilityTimer -= Time.deltaTime;
            
            if (spriteRenderer != null)
            {
                float alpha = Mathf.PingPong(Time.time * 10f, 1f);
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }

            if (_invulnerabilityTimer <= 0f)
            {
                EndInvulnerability();
            }
        }

        private void EndInvulnerability()
        {
            _isInvulnerable = false;
            
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isInvulnerable) return;
            
            TakeDamage(1);
            
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.InstantDestroy();
            }
        }
    }
}