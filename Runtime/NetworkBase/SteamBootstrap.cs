using UnityEngine;
using Steamworks;

namespace NetworkBase
{
    public class SteamBootstrap : MonoBehaviour
    {
        [SerializeField] private uint _appId = 480;

        private void Awake()
        {
#if !UNITY_EDITOR
        try
        {
            SteamClient.Init(_appId, true);
            Debug.Log($"[Steam] Initialized with AppID {_appId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Steam] Initialization failed: " + e);
        }
#endif
        }

        private void OnApplicationQuit()
        {
#if !UNITY_EDITOR
        try
        {
            SteamClient.Shutdown();
            Debug.Log("[Steam] Shutdown");
        }
        catch { }
#endif
        }
    }
}