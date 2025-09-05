using UnityEngine;

namespace AsteroidsModern.Interfaces
{
    public interface IProjectile
    {
        void Initialize(Vector2 direction, float speed, int damage = 1, float lifetime = 3f); 
    }
}