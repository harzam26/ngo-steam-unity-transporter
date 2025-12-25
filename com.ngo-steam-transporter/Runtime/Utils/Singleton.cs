using UnityEngine;

namespace Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _isQuitting;

        public static T Instance
        {
            get
            {
                if(_isQuitting) return null;
                if(_instance == null) _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance && _instance != this)
            {
                Debug.LogWarning($"Duplicate {typeof(T).Name} detected. Destroying extra instance.");
                Destroy(gameObject);
                return;
            }
            
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if(_instance == this) _instance = null;
        }
    }
}