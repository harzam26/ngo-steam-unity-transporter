using Unity.Netcode;
using UnityEngine;

namespace NetworkBase
{
    public class NetcodeLogger : MonoBehaviour
    {
        void Start()
        {
            var nm = NetworkManager.Singleton;
            nm.OnServerStarted += () => Debug.Log("[Netcode] Server Started");
            nm.OnClientConnectedCallback += id => Debug.Log("[Netcode] Client Connected: " + id);
            nm.OnClientDisconnectCallback += id => Debug.Log("[Netcode] Client Disconnected: " + id);
        }
    }
}