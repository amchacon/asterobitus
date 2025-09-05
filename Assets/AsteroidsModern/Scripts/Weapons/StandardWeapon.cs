using AsteroidsModern.Core;
using AsteroidsModern.Enums;
using AsteroidsModern.Interfaces;
using UnityEngine;

namespace AsteroidsModern.Weapons
{
    public class StandardWeapon : WeaponBase
    {
        public StandardWeapon(IProjectileFactory factory, GameSettings settings) : base(factory, settings)
        {
            cooldown = gameSettings.standardShootCooldown;
        }

        public override void Fire(Vector2 origin, Vector2 direction)
        {
            if (!CanFire) return;
            
            var projectile = projectileFactory.CreateProjectile(ProjectileType.Standard, origin);
            float speed = gameSettings?.standardProjectileSpeed ?? 10f;
            int damage = gameSettings?.standardProjectileDamage ?? 2;
            float lifetime = gameSettings?.standardProjectileLifetime ?? 3f;
            projectile.Initialize(direction, speed, damage, lifetime);

            cooldownTimer = cooldown;
            GameEvents.TriggerProjectileFired(origin);
        }
    }
}