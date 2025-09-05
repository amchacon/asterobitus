using System.Collections.Generic;
using UnityEngine;

namespace AsteroidsModern.Core
{
    public class ObjectPool<T> where T : Component
    {
        private readonly Queue<T> _pool = new();
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly int _initialSize;
        private readonly int _maxSize;

        public ObjectPool(GameObject prefab, Transform parent = null, int initialSize = 10, int maxSize = 100)
        {
            _prefab = prefab ?? throw new System.ArgumentNullException(nameof(prefab));
            _parent = parent;
            _initialSize = initialSize;
            _maxSize = maxSize;

            CreateInitialObjects();
        }

        private void CreateInitialObjects()
        {
            for (int i = 0; i < _initialSize; i++)
            {
                CreateNewObject();
            }
        }

        private T CreateNewObject()
        {
            GameObject instance = Object.Instantiate(_prefab, _parent);
            T component = instance.GetComponent<T>();
            
            if (component == null)
            {
                Debug.LogError($"ObjectPool: Prefab {_prefab.name} missing component {typeof(T).Name}");
                Object.Destroy(instance);
                return null;
            }

            instance.SetActive(false);
            _pool.Enqueue(component);
            return component;
        }

        public T Get()
        {
            T obj;
            
            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else
            {
                obj = CreateNewObject();
                if (obj == null) return null;
            }
            
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            if (obj == null) return;
            
            obj.gameObject.SetActive(false);
            obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            
            if (_pool.Count < _maxSize)
            {
                _pool.Enqueue(obj);
            }
            else
            {
                Object.Destroy(obj.gameObject);
            }
        }

        public void Clear()
        {
            while (_pool.Count > 0)
            {
                T obj = _pool.Dequeue();
                if (obj != null)
                    Object.Destroy(obj.gameObject);
            }
        }

        public int AvailableCount => _pool.Count;
        public int MaxSize => _maxSize;
    }
}