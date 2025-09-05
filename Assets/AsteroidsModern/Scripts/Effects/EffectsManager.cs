using System.Collections;
using System.Collections.Generic;
using AsteroidsModern.Core;
using AsteroidsModern.Enums;
using AsteroidsModern.Extensions;
using AsteroidsModern.Interfaces;
using AsteroidsModern.Player;
using UnityEngine;

namespace AsteroidsModern.Scripts.Effects
{
    [System.Serializable]
    public class EffectData
    {
        public string effectName;
        public GameObject effectPrefab;
        public float defaultDuration = 1f;
        public bool useObjectPool = true;
    }

    public class EffectsManager : Singleton<EffectsManager>
    {
        [Header("Effect Prefabs")]
        [SerializeField] private EffectData[] effects;
        
        [Header("Pools")]
        [SerializeField] private int poolSize = 20;

        private Transform _effectsParent;
        
        private readonly Dictionary<string, EffectData> _effectDatabase = new();
        private readonly Dictionary<string, ObjectPool<ParticleSystem>> _effectPools = new();

        private void Awake()
        {
            SubscribeToEvents();
        }

        private void Start()
        {
            InitializeEffectsManager();
        }

        private void InitializeEffectsManager()
        {
            if (_effectsParent == null)
            {
                GameObject parentGo = new GameObject("Effects");
                _effectsParent = parentGo.transform;
                _effectsParent.SetParent(transform);
            }

            BuildEffectDatabase();
            CreateEffectPools();
        }

        private void BuildEffectDatabase()
        {
            _effectDatabase.Clear();
            foreach (var effectData in effects)
            {
                if (!string.IsNullOrEmpty(effectData.effectName) && effectData.effectPrefab != null)
                {
                    _effectDatabase[effectData.effectName] = effectData;
                }
            }
        }

        private void CreateEffectPools()
        {
            foreach (var kvp in _effectDatabase)
            {
                var effectData = kvp.Value;
                if (effectData.useObjectPool && effectData.effectPrefab.GetComponent<ParticleSystem>() != null)
                {
                    GameObject poolParent = new GameObject($"Pool_{effectData.effectName}");
                    poolParent.transform.SetParent(_effectsParent);

                    var pool = new ObjectPool<ParticleSystem>(
                        effectData.effectPrefab,
                        poolParent.transform,
                        poolSize / 4,
                        poolSize
                    );

                    _effectPools[effectData.effectName] = pool;
                }
            }
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnAsteroidHit += OnAsteroidHit;
            GameEvents.OnAsteroidDestroyed += OnAsteroidDestroyed;
            GameEvents.OnPlayerDied += PlayerDied;
            GameEvents.OnProjectileFired += OnProjectileFired;
        }

        protected override void OnDestroy()
        {
            GameEvents.OnAsteroidHit -= OnAsteroidHit;
            GameEvents.OnAsteroidDestroyed -= OnAsteroidDestroyed;
            GameEvents.OnPlayerDied -= PlayerDied;
            GameEvents.OnProjectileFired -= OnProjectileFired;
        }

        private void OnAsteroidHit(Vector2 hitPoint)
        {
            PlaySpriteEffect("asteroid_hit", hitPoint, 0.1f);
            ShakeScreen(0.1f, 0.1f);
        }

        private void OnAsteroidDestroyed(IAsteroid asteroid)
        {
            string effectName = asteroid.Size switch
            {
                AsteroidSize.Large => "large_asteroid_explosion",
                AsteroidSize.Medium => "medium_asteroid_explosion",
                AsteroidSize.Small => "small_asteroid_explosion",
                _ => "asteroid_explosion"
            };

            PlaySpriteEffect(effectName, asteroid.Position);
        }

        private void PlayerDied()
        {
            var player = FindFirstObjectByType<PlayerHealth>();
            if (player != null)
            {
                PlayEffect("player_explosion", player.transform.position, 2f);
            }
        }

        private void OnProjectileFired(Vector2 position)
        {
            PlayEffect("muzzle_flash", position, 0.2f);
        }
        
        void PlaySpriteEffect (string effectName, Vector2 position, float duration = -1f)
        {
            if (!_effectDatabase.TryGetValue(effectName, out var effectData))
            {
                Debug.LogWarning($"EffectsManager: Effect '{effectName}' not found!");
                return;
            }

            float effectDuration = duration > 0 ? duration : effectData.defaultDuration;

            GameObject effectInstance = Instantiate(effectData.effectPrefab, position, Quaternion.identity, _effectsParent);
            var spriteRenderer = effectInstance.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 10; // Ensure it's rendered above other elements
                spriteRenderer.enabled = true;
            }

            Destroy(effectInstance, effectDuration);
        }

        public void PlayEffect(string effectName, Vector2 position, float duration = -1f)
        {
            if (!_effectDatabase.ContainsKey(effectName))
            {
                Debug.LogWarning($"EffectsManager: Effect '{effectName}' not found!");
                return;
            }

            var effectData = _effectDatabase[effectName];
            float effectDuration = duration > 0 ? duration : effectData.defaultDuration;

            if (effectData.useObjectPool && _effectPools.ContainsKey(effectName))
            {
                PlayPooledEffect(effectName, position, effectDuration);
            }
            else
            {
                PlayInstantiatedEffect(effectData, position, effectDuration);
            }
        }

        private void PlayPooledEffect(string effectName, Vector2 position, float duration)
        {
            var pool = _effectPools[effectName];
            var effect = pool.Get();
            
            if (effect != null)
            {
                effect.transform.position = position;
                effect.Play();
                
                StartCoroutine(ReturnEffectToPool(effect, pool, duration));
            }
        }

        private void PlayInstantiatedEffect(EffectData effectData, Vector2 position, float duration)
        {
            GameObject effectInstance = Instantiate(effectData.effectPrefab, position, Quaternion.identity, _effectsParent);
            
            var particleSystem = effectInstance.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            Destroy(effectInstance, duration);
        }

        private IEnumerator ReturnEffectToPool(ParticleSystem effect, ObjectPool<ParticleSystem> pool, float duration)
        {
            yield return new WaitForSeconds(duration);
            
            if (effect != null)
            {
                effect.Stop();
                effect.Clear();
                pool.Return(effect);
            }
        }

        public void SetEffectPrefab(string effectName, GameObject prefab)
        {
            if (_effectDatabase.ContainsKey(effectName))
            {
                _effectDatabase[effectName].effectPrefab = prefab;
            }
        }

        public void SetEffectDuration(string effectName, float duration)
        {
            if (_effectDatabase.ContainsKey(effectName))
            {
                _effectDatabase[effectName].defaultDuration = duration;
            }
        }

        public void StopAllEffects()
        {
            foreach (var pool in _effectPools.Values)
            {
                // Implement method to stop all active effects in the pool if needed
            }

            var childEffects = _effectsParent.GetComponentsInChildren<ParticleSystem>();
            foreach (var effect in childEffects)
            {
                if (effect.isPlaying)
                    effect.Stop();
            }
        }

        public bool HasEffect(string effectName)
        {
            return _effectDatabase.ContainsKey(effectName);
        }

        public void ShakeScreen(float intensity = 0.5f, float duration = 0.3f)
        {
            var camera = Camera.main;
            if (camera != null)
            {
                StartCoroutine(ScreenShakeCoroutine(camera, intensity, duration));
            }
        }

        private IEnumerator ScreenShakeCoroutine(Camera camera, float intensity, float duration)
        {
            Vector3 originalPosition = camera.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;

                camera.transform.position = originalPosition + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            camera.transform.position = originalPosition;
        }
    }
}