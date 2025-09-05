using System.Collections.Generic;
using AsteroidsModern.Extensions;
using UnityEngine;

namespace AsteroidsModern.Core
{
    public class PoolManager : Singleton<PoolManager>
    {
        [System.Serializable]
        public class PoolConfig
        {
            public string poolName;
            public GameObject prefab;
            public int initialSize = 10;
            public int maxSize = 50;
        }

        [SerializeField] private PoolConfig[] poolConfigs;
        [SerializeField] private Transform poolParent;

        private readonly Dictionary<string, ObjectPool<Transform>> _pools = new();
        private readonly Dictionary<Transform, string> _objectToPoolMap = new();

        private void Awake()
        {
            if (poolParent == null)
            {
                GameObject poolParentGo = new GameObject("Object Pools");
                poolParent = poolParentGo.transform;
                poolParent.SetParent(transform);
            }

            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var config in poolConfigs)
            {
                if (config.prefab == null)
                {
                    Debug.LogWarning($"GameObjectPoolManager: Prefab not defined for pool '{config.poolName}'");
                    continue;
                }
                
                GameObject poolGo = new GameObject($"Pool_{config.poolName}");
                poolGo.transform.SetParent(poolParent);

                var pool = new ObjectPool<Transform>(
                    config.prefab, 
                    poolGo.transform, 
                    config.initialSize, 
                    config.maxSize
                );

                _pools[config.poolName] = pool;
            }
        }

        public T GetFromPool<T>(string poolName) where T : Component
        {
            if (!_pools.TryGetValue(poolName, out var pool))
            {
                Debug.LogError($"GameObjectPoolManager: Pool '{poolName}' not found!");
                return null;
            }

            Transform obj = pool.Get();
            if (obj == null) return null;

            _objectToPoolMap[obj] = poolName;
            return obj.GetComponent<T>();
        }

        public void ReturnToPool(GameObject obj)
        {
            if (obj == null) return;

            Transform objTransform = obj.transform;
            if (_objectToPoolMap.ContainsKey(objTransform))
            {
                string poolName = _objectToPoolMap[objTransform];
                _objectToPoolMap.Remove(objTransform);
                
                if (_pools.TryGetValue(poolName, out var pool))
                {
                    pool.Return(objTransform);
                }
            }
            else
            {
                Destroy(obj);
            }
        }

        public void ClearPool(string poolName)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                pool.Clear();
            }
        }

        public void ClearAllPools()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }
            _objectToPoolMap.Clear();
        }

        public int GetPoolAvailableCount(string poolName)
        {
            return _pools.TryGetValue(poolName, out var pool) ? pool.AvailableCount : 0;
        }
    }
}