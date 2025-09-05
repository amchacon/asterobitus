using System;
using AsteroidsModern.Enums;
using AsteroidsModern.Interfaces;
using AsteroidsModern.Weapons;
using UnityEngine;

namespace AsteroidsModern.Core
{
    public static class GameEvents
    {
        public static event Action<int> OnScoreChanged;

        public static event Action<int> OnPlayerHealthChanged;
        public static event Action OnPlayerDied;

        public static event Action<Vector2> OnAsteroidHit;
        public static event Action<IAsteroid> OnAsteroidDestroyed;
        public static event Action<IAsteroid> OnAsteroidSpawned;
        
        public static event Action<GameState> OnGameStateChanged;
        public static event Action OnGameStarted;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action OnGameOver;

        public static event Action<WeaponBase> OnWeaponChanged;
        public static event Action<Vector2> OnProjectileFired;

        public static void TriggerScoreChanged(int newScore)
        {
            OnScoreChanged?.Invoke(newScore);
        }
        
        public static void TriggerPlayerHealthChanged(int newHealth)
        {
            OnPlayerHealthChanged?.Invoke(newHealth);
        }

        public static void TriggerPlayerDestroyed()
        {
            OnPlayerDied?.Invoke();
        }
        
        public static void TriggerAsteroidHit(Vector2 hitPoint)
        {
            OnAsteroidHit?.Invoke(hitPoint);
        }

        public static void TriggerAsteroidDestroyed(IAsteroid asteroid)
        {
            OnAsteroidDestroyed?.Invoke(asteroid);
        }

        public static void TriggerAsteroidSpawned(IAsteroid asteroid)
        {
            OnAsteroidSpawned?.Invoke(asteroid);
        }

        public static void TriggerGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);
        }

        public static void TriggerGameStarted()
        {
            OnGameStarted?.Invoke();
        }

        public static void TriggerGamePaused()
        {
            OnGamePaused?.Invoke();
        }

        public static void TriggerGameResumed()
        {
            OnGameResumed?.Invoke();
        }

        public static void TriggerGameOver()
        {
            OnGameOver?.Invoke();
        }
        
        public static void TriggerWeaponChanged(WeaponBase weapon)
        {
            OnWeaponChanged?.Invoke(weapon);
        }

        public static void TriggerProjectileFired(Vector2 position)
        {
            OnProjectileFired?.Invoke(position);
        }

        public static void ClearAllEvents()
        {
            OnScoreChanged = null;
            OnPlayerHealthChanged = null;
            OnPlayerDied = null;
            OnAsteroidDestroyed = null;
            OnAsteroidSpawned = null;
            OnGameStateChanged = null;
            OnGameStarted = null;
            OnGamePaused = null;
            OnGameResumed = null;
            OnGameOver = null;
            OnWeaponChanged = null;
            OnProjectileFired = null;
        }
    }
}