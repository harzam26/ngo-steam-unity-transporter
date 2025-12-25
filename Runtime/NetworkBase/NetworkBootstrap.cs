using System.Threading.Tasks;
using Netcode.Transports.Facepunch;
using Steamworks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkBase
{
    public class NetworkBootstrap : MonoBehaviour
    {
        public static NetworkBootstrap Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private string _gameSceneName = "GameScene";
        [SerializeField] private bool _useRelay = true; // Editörde Relay kullanılsın mı?
        
        public string GameSceneName => _gameSceneName;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            // Unity Services Başlatma (Relay için şart)
            try
            {
                if (UnityServices.State == ServicesInitializationState.Uninitialized)
                {
                    await UnityServices.InitializeAsync();
                    if (!AuthenticationService.Instance.IsSignedIn)
                        await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Unity Services Init Error: {e}");
            }
        }

        // --- HOST ---
        public async Task<string> StartHostAsync()
        {
            var nm = NetworkManager.Singleton;
            // Transport tipini otomatik algıla
            var transport = nm.NetworkConfig.NetworkTransport;
            string joinCode = "ERROR";

            if (transport is UnityTransport utp)
            {
                if (_useRelay)
                {
                    try
                    {
                        var allocation = await RelayService.Instance.CreateAllocationAsync(4);
                        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                        utp.SetRelayServerData(allocation.ToRelayServerData("dtls"));
                    }
                    catch (System.Exception e) { Debug.LogError($"Relay Create Error: {e}"); }
                }
                else
                {
                    joinCode = "LOCAL-IP"; // Relay kapalıysa direkt IP
                    utp.SetConnectionData("127.0.0.1", 7777);
                }
            }
            else if (transport is FacepunchTransport) // STEAM
            {
                // Steam Lobi mantığı
                var lobby = await SteamMatchmaking.CreateLobbyAsync(4);
                if (lobby.HasValue)
                {
                    lobby.Value.SetPublic();
                    lobby.Value.SetJoinable(true);
                    lobby.Value.SetData("host_steam_id", SteamClient.SteamId.Value.ToString());
                    joinCode = lobby.Value.Id.Value.ToString(); // Uzun Steam ID
                }
            }

            if (nm.StartHost())
            {
                return joinCode;
            }
            return null;
        }

        // --- JOIN ---
        public async Task<bool> StartClientAsync(string joinCode)
        {
            var nm = NetworkManager.Singleton;
            var transport = nm.NetworkConfig.NetworkTransport;

            if (transport is UnityTransport utp)
            {
                if (_useRelay && joinCode != "LOCAL-IP")
                {
                    try
                    {
                        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                        utp.SetRelayServerData(allocation.ToRelayServerData("dtls"));
                    }
                    catch { return false; }
                }
                else
                {
                    utp.SetConnectionData("127.0.0.1", 7777);
                }
            }
            else if (transport is FacepunchTransport fp)
            {
                if (ulong.TryParse(joinCode, out ulong lobbyId))
                {
                    var lobby = await SteamMatchmaking.JoinLobbyAsync(lobbyId);
                    if (lobby.HasValue)
                    {
                        string hostId = lobby.Value.GetData("host_steam_id");
                        fp.targetSteamId = ulong.Parse(hostId);
                    }
                }
            }

            return nm.StartClient();
        }

        public void StartGame()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(_gameSceneName, LoadSceneMode.Single);
            }
        }
    }
}