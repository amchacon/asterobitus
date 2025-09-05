using AsteroidsModern.Enums;
using UnityEngine;

namespace AsteroidsModern.Interfaces
{
    public interface IProjectileFactory
    {
        IProjectile CreateProjectile(ProjectileType type, Vector2 position);
    }
}