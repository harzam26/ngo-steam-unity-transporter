using System;
using Steamworks;
using UnityEngine;
using Utils;

namespace Menu
{
    public class SteamManager : Singleton<SteamManager>
    {
        public SteamId MySteamID { get; private set; }
        public string MyName { get; private set; }
        [SerializeField] private uint _steamAppID;

        private void Start()
        {
            #if UNITY_EDITOR
            try
            {
                SteamClient.Init(_steamAppID, true);

                MySteamID = SteamClient.SteamId;
                MyName = SteamClient.Name;
                Debug.Log($"Steam initialized: {MyName} ({MySteamID})");
            }
            catch (Exception e)
            {
                Debug.LogError($"Steam init error: {e.Message}");
            }
            #endif
        }

        private void Update()
        {
            SteamClient.RunCallbacks();
        }
    }
}