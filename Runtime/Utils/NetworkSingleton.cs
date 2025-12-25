using Unity.Netcode;
using UnityEngine;

namespace Utils
{
    public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        private static T _instance;
        private static bool _isQuitting;
        public virtual bool WillDestroyOnLoad => false;
        
        public static T Instance
        {
            get
            {
                if (_isQuitting) return null;
                if (!_instance) _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                return _instance;
            }
        }

        public override void OnNetworkSpawn()
        {
            if (_instance && _instance != this)
            {
                Debug.LogWarning($"Duplicate {typeof(T).Name} detected. Destroying extra instance.");
                Destroy(gameObject);
                return;
            }
            
            _instance = this as T;
            if (!WillDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
                
            }
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            if(_instance == this) _instance = null;
            base.OnNetworkDespawn();
        }

        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
        }
    }
}