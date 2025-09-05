using AsteroidsModern.Core;
using AsteroidsModern.Interfaces;
using UnityEngine;

namespace AsteroidsModern.Weapons
{
    public abstract class WeaponBase
    {
        protected float cooldownTimer;
        protected readonly IProjectileFactory projectileFactory;
        protected readonly GameSettings gameSettings; 
        public bool CanFire => cooldownTimer <= 0f;
        public float cooldown; 
        
        protected WeaponBase(IProjectileFactory factory, GameSettings settings)
        {
            projectileFactory = factory;
            gameSettings = settings;
        }
        
        public abstract void Fire(Vector2 origin, Vector2 direction);
        
        public virtual void UpdateCooldown(float deltaTime)
        { 
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= deltaTime;
            }
        }
    }
}