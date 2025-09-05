using UnityEngine;

namespace AsteroidsModern.Extensions
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationQuitting = false;

        public static T Instance
        {
            get
            {
                if (_applicationQuitting) return null;

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindFirstObjectByType(typeof(T));

                        if (_instance == null)
                        {
                            var singletonObj = new GameObject(typeof(T).Name);
                            _instance = singletonObj.AddComponent<T>();
                        }
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                    return _instance;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            _applicationQuitting = true;
        }
    }
}