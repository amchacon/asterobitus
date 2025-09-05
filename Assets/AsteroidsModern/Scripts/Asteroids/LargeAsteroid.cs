using AsteroidsModern.Enums;
using UnityEngine;

namespace AsteroidsModern.Scripts.Asteroids
{
    public class LargeAsteroid : AsteroidBase
    {
        protected override void ConfigureForSize()
        {
            transform.localScale = Vector3.one * 0.4f;
        }

        protected override void DestroyAsteroid()
        {
            SpawnSmallerAsteroids();
            base.DestroyAsteroid();
        }

        private void SpawnSmallerAsteroids()
        {
            Vector2 currentPos = transform.position;
            Vector2 currentVel = rigid.linearVelocity;
            
            Vector2 perpendicular = Vector2.Perpendicular(currentVel.normalized);
            Vector2 direction1 = (currentVel.normalized + perpendicular * 0.5f).normalized;
            Vector2 direction2 = (currentVel.normalized - perpendicular * 0.5f).normalized;

            var medium1 = spawner.SpawnAsteroid(AsteroidSize.Medium, currentPos + direction1 * 0.5f, direction1);
            var medium2 = spawner.SpawnAsteroid(AsteroidSize.Medium, currentPos + direction2 * 0.5f, direction2);

            medium1?.Initialize(spawner, AsteroidSize.Medium, direction1);
            medium2?.Initialize(spawner, AsteroidSize.Medium, direction2);
        }
    }
}