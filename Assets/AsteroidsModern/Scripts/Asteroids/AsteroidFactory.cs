using AsteroidsModern.Enums;
using AsteroidsModern.Interfaces;
using UnityEngine;

namespace AsteroidsModern.Scripts.Asteroids
{
    public class AsteroidFactory : MonoBehaviour, IAsteroidFactory
    {
        [SerializeField] private GameObject largeAsteroidPrefab;
        [SerializeField] private GameObject mediumAsteroidPrefab;
        [SerializeField] private GameObject smallAsteroidPrefab;
        
        private Transform _asteroidParent;
        
        private void Start()
        {
            if (_asteroidParent == null) 
            {
                GameObject parentGo = new GameObject("Asteroids");
                _asteroidParent = parentGo.transform;
                _asteroidParent.SetParent(transform);
            }
        }

        public IAsteroid CreateAsteroid(AsteroidSize size, Vector2 position)
        {
            GameObject prefab = GetAsteroidPrefab(size);
            if (prefab == null)
            {
                return null;
            }

            GameObject instance = Instantiate(prefab, position, Quaternion.identity, _asteroidParent);
            var asteroid = instance.GetComponent<IAsteroid>();
            return asteroid;
        }

        private GameObject GetAsteroidPrefab(AsteroidSize size)
        {
            return size switch
            {
                AsteroidSize.Large => largeAsteroidPrefab,
                AsteroidSize.Medium => mediumAsteroidPrefab,
                _ => smallAsteroidPrefab
            };
        }
    }
}