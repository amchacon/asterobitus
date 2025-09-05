using AsteroidsModern.Scripts.Asteroids;
using AsteroidsModern.Enums;
using UnityEngine;

namespace AsteroidsModern.Interfaces
{
    public interface IAsteroid : IDamageable
    {
        AsteroidSize Size { get; }
        int ScoreValue { get; }
        Vector3 Position { get; }
        void Initialize(AsteroidSpawner spawner, AsteroidSize size, Vector2 direction);
    }
}