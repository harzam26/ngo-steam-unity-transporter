using Netcode.Transports.Facepunch;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace NetworkBase
{
    [DefaultExecutionOrder(-10000)]
    public class TransportSelector : MonoBehaviour
    {
        public enum Mode
        {
            UNITY,
            FACE_PUNCH,
            AUTO
        }

        [SerializeField] private Mode _mode = Mode.AUTO;

        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private UnityTransport _unityTransport;
        [SerializeField] private FacepunchTransport _facepunchTransport;
        [SerializeField] private bool _forceUnityInBuild = false;

        public Mode CurrentMode => _mode;

        void Awake()
        {
            var pick = PickTransport();
            // yalnız tek transport aktif olsun
            if (_unityTransport) _unityTransport.enabled = (pick == _unityTransport);
            if (_facepunchTransport) _facepunchTransport.enabled = (pick == _facepunchTransport);
            _networkManager.NetworkConfig.NetworkTransport = pick;
            Debug.Log("[TransportSelector] Active = " + pick.GetType().Name);
        }

        NetworkTransport PickTransport()
        {
            if (_mode == Mode.UNITY) return _unityTransport;
            if (_mode == Mode.FACE_PUNCH) return _facepunchTransport;

            // Auto
#if UNITY_EDITOR
            return _unityTransport; // editörde local test
#else
        return _forceUnityInBuild ? _unityTransport : _facepunchTransport;
#endif
        }
    }
}