using NetworkBase;
using Unity.Netcode;
using Utils;

namespace Menu
{
    public class LobbyManager : NetworkSingleton<LobbyManager>
    {
        // Oyuncuları takip etmek için liste
        private readonly NetworkList<ulong> _connectedPlayers = new NetworkList<ulong>();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
                _connectedPlayers.Add(NetworkManager.LocalClientId); // Host'u ekle
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            _connectedPlayers.Add(clientId);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (_connectedPlayers.Contains(clientId))
                _connectedPlayers.Remove(clientId);
        }
        
        // Bootstrap üzerinden Host başlatır
        public async void HostGame(System.Action<string> onCodeReady)
        {
            string code = await NetworkBootstrap.Instance.StartHostAsync();
            onCodeReady?.Invoke(code);
        }

        // Bootstrap üzerinden Join işlemini başlatır
        public async void JoinGame(string code, System.Action<bool> onResult)
        {
            bool success = await NetworkBootstrap.Instance.StartClientAsync(code);
            onResult?.Invoke(success);
        }

        public void StartGame()
        {
            if (!IsHost) return;
            
            // Oyun sahnesine geçiş
            NetworkBootstrap.Instance.StartGame();
        }
        
        public int GetPlayerCount() => _connectedPlayers.Count;
    }
}