using UnityEngine;

namespace AsteroidsModern.Core
{
    [System.Serializable]
    public class GameSettings
    {
        [Header("Player Settings")]
        public float playerMoveSpeed;
        public float playerRotationSpeed;
        public int playerMaxHealth;
        public float playerInvulnerabilityTime;

        [Header("Standard Projectile Settings")]
        public float standardShootCooldown;
        public float standardProjectileSpeed;
        public float standardProjectileLifetime;
        public int standardProjectileDamage;
        
        [Header("Double Projectile Settings")]
        public float doubleShootCooldown;
        public float doubleProjectileSpeed;
        public float doubleProjectileLifetime;
        public int doubleProjectileDamage;

        [Header("Large Asteroid Settings")]
        public float largeAsteroidSpeed;
        public int largeAsteroidHealth;
        public int largeAsteroidScore;
        
        [Header("Medium Asteroid Settings")]
        public float mediumAsteroidSpeed;
        public int mediumAsteroidHealth;
        public int mediumAsteroidScore;
        
        [Header("Small Asteroid Settings")]
        public float smallAsteroidSpeed;
        public int smallAsteroidHealth;
        public int smallAsteroidScore;
        
        [Header("Spawn Settings")]
        public float initialSpawnRate;
        public float spawnRateIncrease;
        public int maxAsteroidsOnScreen;
    }
}