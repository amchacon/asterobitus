using AsteroidsModern.Enums;
using UnityEngine;

namespace AsteroidsModern.Scripts.Asteroids
{
    public class MediumAsteroid : AsteroidBase
    {
        protected override void ConfigureForSize()
        {
            transform.localScale = Vector3.one * 0.25f;
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
            Vector2 direction1 = (currentVel.normalized + perpendicular * 0.7f).normalized;
            Vector2 direction2 = (currentVel.normalized - perpendicular * 0.7f).normalized;

            var small1 = spawner.SpawnAsteroid(AsteroidSize.Small, currentPos + direction1 * 0.3f, direction1);
            var small2 = spawner.SpawnAsteroid(AsteroidSize.Small, currentPos + direction2 * 0.3f, direction2);

            small1?.Initialize(spawner, AsteroidSize.Small, direction1);
            small2?.Initialize(spawner, AsteroidSize.Small, direction2);
        }
    }
}