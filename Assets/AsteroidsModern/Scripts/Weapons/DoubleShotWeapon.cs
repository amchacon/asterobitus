using System;
using AsteroidsModern.Core;
using AsteroidsModern.Enums;
using AsteroidsModern.Interfaces;
using UnityEngine;

namespace AsteroidsModern.Weapons
{
    public class DoubleShotWeapon : WeaponBase
    {
        private readonly float _spreadAngle;

        public DoubleShotWeapon(IProjectileFactory factory, GameSettings settings, float spreadAngle = 15f) : base(factory, settings)
        {
            _spreadAngle = spreadAngle;
            cooldown = gameSettings.doubleShootCooldown;
        }

        public override void Fire(Vector2 origin, Vector2 direction)
        {
            if (!CanFire) return;

            float angleRad = Mathf.Atan2(direction.y, direction.x);
            float leftAngle = angleRad + (_spreadAngle * Mathf.Deg2Rad);
            float rightAngle = angleRad - (_spreadAngle * Mathf.Deg2Rad);

            Vector2 leftDirection = new Vector2(Mathf.Cos(leftAngle), Mathf.Sin(leftAngle));
            Vector2 rightDirection = new Vector2(Mathf.Cos(rightAngle), Mathf.Sin(rightAngle));

            var leftProjectile = projectileFactory.CreateProjectile(ProjectileType.Standard, origin);
            var rightProjectile = projectileFactory.CreateProjectile(ProjectileType.Standard, origin);

            float speed = gameSettings?.doubleProjectileSpeed ?? 6f;
            int damage = gameSettings?.doubleProjectileDamage ?? 1;
            float lifetime = gameSettings?.doubleProjectileLifetime ?? 1.5f;
            
            leftProjectile.Initialize(leftDirection, speed, damage, lifetime);
            rightProjectile.Initialize(rightDirection, speed, damage, lifetime);

            cooldownTimer = cooldown;
            GameEvents.TriggerProjectileFired(origin);
        }
    }
}