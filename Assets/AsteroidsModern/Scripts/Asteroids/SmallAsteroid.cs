using UnityEngine;

namespace AsteroidsModern.Scripts.Asteroids
{
    public class SmallAsteroid : AsteroidBase
    {
        protected override void ConfigureForSize()
        {
            transform.localScale = Vector3.one * 0.15f;
        }
    }
}