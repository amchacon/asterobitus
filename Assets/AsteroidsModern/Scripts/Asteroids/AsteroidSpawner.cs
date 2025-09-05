using System.Collections;
using System.Collections.Generic;
using AsteroidsModern.Core;
using AsteroidsModern.Enums;
using AsteroidsModern.Interfaces;
using AsteroidsModern.Managers;
using AsteroidsModern.Extensions;
using AsteroidsModern.Player;
using UnityEngine;

namespace AsteroidsModern.Scripts.Asteroids
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnRate = 2f;
        [SerializeField] private float spawnRateIncrease = 0.1f;
        [SerializeField] private int maxAsteroidsOnScreen = 10;
        [SerializeField] private float minDistanceFromPlayer = 3f;
        [SerializeField] private int initialAsteroidCount = 3;
        [SerializeField] private bool isSpawning = true;
        [SerializeField] private bool useRandomSpeed = true;

        private IAsteroidFactory _asteroidFactory;
        private GameSettings _settings;
        private Camera _mainCamera;
        private Transform _playerTransform;
        private readonly List<IAsteroid> _activeAsteroids = new();
        private Coroutine _spawnCoroutine;
        private float _currentSpawnRate;

        private void Awake()
        {
            SubscribeToEvents();
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            _playerTransform = FindFirstObjectByType<PlayerMovement>()?.transform;
            
            InitializeFactory();
            ApplyConfiguration(ConfigLoader.Instance.GameSettings);
        }

        private void InitializeFactory()
        {
            _asteroidFactory = GetComponent<AsteroidFactory>() ?? gameObject.AddComponent<AsteroidFactory>();
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnAsteroidDestroyed += OnAsteroidDestroyed;
            GameEvents.OnAsteroidSpawned += OnAsteroidSpawned;
            GameEvents.OnGameStarted += OnGameStarted;
            GameEvents.OnGameOver += OnGameOver;
            GameEvents.OnGamePaused += StopSpawning;
            GameEvents.OnGameResumed += StartSpawning;
        }

        private void OnDestroy()
        {
            GameEvents.OnAsteroidDestroyed -= OnAsteroidDestroyed;
            GameEvents.OnAsteroidSpawned -= OnAsteroidSpawned;
            GameEvents.OnGameStarted -= OnGameStarted;
            GameEvents.OnGameOver -= OnGameOver;
            GameEvents.OnGamePaused -= StopSpawning;
            GameEvents.OnGameResumed -= StartSpawning;
        }

        private void OnGameStarted()
        {
            ClearAllAsteroids();
            SpawnInitialAsteroids(initialAsteroidCount);
            StartSpawning();
        }
        
        private void OnGameOver()
        {
            StopSpawning();
            ClearAllAsteroids();
        }
        
        private void StartSpawning()
        {
            if (isSpawning) return;

            isSpawning = true;
            _currentSpawnRate = spawnRate;
            
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }
            
            _spawnCoroutine = StartCoroutine(SpawnRoutine());
        }

        private void StopSpawning()
        {
            isSpawning = false;
            
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnRoutine()
        {
            while (isSpawning)
            {
                if (_activeAsteroids.Count < maxAsteroidsOnScreen)
                {
                    SpawnRandomAsteroid();
                    
                    _currentSpawnRate = Mathf.Max(0.5f, _currentSpawnRate - spawnRateIncrease * Time.deltaTime);
                }

                yield return new WaitForSeconds(_currentSpawnRate);
            }
        }

        private void SpawnRandomAsteroid()
        {
            Vector2 spawnPosition = GetRandomSpawnPosition();
            Vector2 spawnDirection = GetRandomDirection();
            AsteroidSize spawnSize = GetRandomSize();

            SpawnAsteroid(spawnSize, spawnPosition, spawnDirection);
        }

        internal IAsteroid SpawnAsteroid(AsteroidSize spawnSize, Vector2 spawnPosition, Vector2 spawnDirection)
        {
            IAsteroid asteroid = _asteroidFactory.CreateAsteroid(spawnSize, spawnPosition);
            if (asteroid != null)
            {
                asteroid.Initialize(this, spawnSize, spawnDirection);
            }

            return asteroid;
        }

        private Vector2 GetRandomSpawnPosition()
        {
            if (_mainCamera == null) return Vector2.zero;

            var spawnPos = Utils.GetRandomEdgePosition(_mainCamera);
            
            if (_playerTransform != null)
            {
                float distanceToPlayer = Vector2.Distance(spawnPos, _playerTransform.position);
                if (distanceToPlayer < minDistanceFromPlayer)
                {
                    return GetRandomSpawnPosition();
                }
            }

            return spawnPos;
        }

        private Vector2 GetRandomDirection()
        {
            Vector2 screenCenter = _mainCamera?.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)) ?? Vector2.zero;
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            
            Vector2 toCenter = screenCenter.normalized;
            Vector2 finalDirection = (randomDirection * 0.7f + toCenter * 0.3f).normalized;
            
            return finalDirection;
        }

        private AsteroidSize GetRandomSize()
        { 
            float random = Random.Range(0f, 1f);
            
            if (random < 0.1f) return AsteroidSize.Small;
            if (random < 0.4f) return AsteroidSize.Medium;
            return AsteroidSize.Large;
        }

        internal float GetSpeedForSize(AsteroidSize size)
        {
            if (_settings == null) return 2f;

            var speed=  size switch
            {
                AsteroidSize.Large => _settings.largeAsteroidSpeed,
                AsteroidSize.Medium => _settings.mediumAsteroidSpeed,
                _ => _settings.smallAsteroidSpeed
            };
            
            if (useRandomSpeed)
            {
                float variation = speed * 0.2f;
                speed += Random.Range(-variation, variation);
            }
            
            return speed;
        }
        
        internal int GetHealthForSize(AsteroidSize size)
        {
            if (_settings == null) return 3;

            return size switch
            {
                AsteroidSize.Large => _settings.largeAsteroidHealth,
                AsteroidSize.Medium => _settings.mediumAsteroidHealth,
                _ => _settings.smallAsteroidHealth
            };
        }
        
        internal int GetScoreForSize(AsteroidSize size)
        {
            if (_settings == null) return 20;

            return size switch
            {
                AsteroidSize.Large => _settings.largeAsteroidScore,
                AsteroidSize.Medium => _settings.mediumAsteroidScore,
                _ => _settings.smallAsteroidScore
            };
        }

        private void OnAsteroidDestroyed(IAsteroid asteroid)
        {
            _activeAsteroids.Remove(asteroid);
        }

        private void OnAsteroidSpawned(IAsteroid asteroid)
        {
            if (!_activeAsteroids.Contains(asteroid))
            {
                _activeAsteroids.Add(asteroid);
            }
        }

        private void ApplyConfiguration(GameSettings settings)
        {
            if (settings == null) return;

            _settings = settings;
            spawnRate = settings.initialSpawnRate;
            spawnRateIncrease = settings.spawnRateIncrease;
            maxAsteroidsOnScreen = settings.maxAsteroidsOnScreen;
        }

        private void ClearAllAsteroids()
        {
            var asteroidsToDestroy = new List<IAsteroid>(_activeAsteroids);
            foreach (var asteroid in asteroidsToDestroy)
            {
                if (asteroid is MonoBehaviour mono && mono != null)
                {
                    Destroy(mono.gameObject);
                }
            }
            _activeAsteroids.Clear();
        }

        private void SpawnInitialAsteroids(int count = 3)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnRandomAsteroid();
            }
        }
    }
}