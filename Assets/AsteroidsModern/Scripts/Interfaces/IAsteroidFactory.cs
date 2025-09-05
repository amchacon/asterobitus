using AsteroidsModern.Enums;
using UnityEngine;

namespace AsteroidsModern.Interfaces
{
    public interface IAsteroidFactory
    {
        IAsteroid CreateAsteroid(AsteroidSize size, Vector2 position);
    }
}