using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace Player
{
    public class GameplayPlayerSpawnManager : NetworkSingleton<GameplayPlayerSpawnManager>
    {
        [Header("References")]
        [SerializeField] private GameObject _playerGameplayPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private string _gameplaySceneName;

        public override void OnNetworkSpawn()
        {
            // Sadece Server (Host) bu işlemi yönetmeli
            if (IsServer)
            {
                // Sahne yüklemesi bittiğinde tetiklenecek eventi dinle
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadEventCompleted;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer && NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoadEventCompleted;
            }
        }

        private void OnSceneLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            // Sadece bizim oyun sahnemiz yüklendiğinde çalışsın
            if (sceneName == _gameplaySceneName) // Sahne adının birebir aynı olduğundan emin ol
            {
                SpawnPlayerPrefabs();
            }
        }

        private void SpawnPlayerPrefabs()
        {
            Debug.Log("[Gameplay] Spawning players...");

            int index = 0; // Array hatasını önlemek için manuel sayaç

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                // Spawn noktası kalmadıysa döngüyü kır (Hata vermesin)
                if (index >= _spawnPoints.Length)
                {
                    Debug.LogWarning("Yeterli Spawn Point yok!");
                    break;
                }

                // Doğru pozisyonu al
                Transform spawnPoint = _spawnPoints[index];

                // Prefab'ı yarat
                var playerInstance = client.PlayerObject;
                
                playerInstance.transform.position = spawnPoint.position;

                index++;
            }
        }
    }
}