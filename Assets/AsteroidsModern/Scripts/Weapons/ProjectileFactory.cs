using AsteroidsModern.Core;
using AsteroidsModern.Enums;
using AsteroidsModern.Interfaces;
using UnityEngine;

namespace AsteroidsModern.Weapons
{
    public class ProjectileFactory : MonoBehaviour, IProjectileFactory
    {
        public IProjectile CreateProjectile(ProjectileType type, Vector2 position)
        {
            string poolKey = GetProjectilePoolKey(type);
            var projectile = PoolManager.Instance.GetFromPool<Projectile>(poolKey);
            projectile.transform.position = position;
            projectile.transform.rotation = Quaternion.identity;
            return projectile;
        }

        private string GetProjectilePoolKey(ProjectileType type)
        {
            return type switch
            {
                ProjectileType.Standard => "StandardProjectile",
                ProjectileType.Explosive => "ExplosiveProjectile",
                ProjectileType.Piercing => "PiercingProjectile",
                _ => "StandardProjectile"
            };
        }
    }
}