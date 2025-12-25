using System.Collections.Generic;
using Menu;
using NetworkBase;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("Visual References")]
        [SerializeField] private GameObject[] _models;
        [SerializeField] private Animator _animator;
        
        private PlayerStateManager _stateManager;

        private void Awake()
        {
            _stateManager = GetComponent<PlayerStateManager>();
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SetCamera;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                // Owner değilsek bu scriptin Update'ine ihtiyacımız yok
                // Ama Locomotion ve NetworkTransform çalışmaya devam etmeli.
                enabled = false; 
                return;
            }

            var lobbyAvatar = FindAnyObjectByType<PlayerLobbyAvatar>();
            if (lobbyAvatar)
            {
                int characterIndex = lobbyAvatar.GetCharacterIndex();
                ApplyCharacterModel(characterIndex);
                // Bu indexi diğer oyunculara da bildirmek için bir ServerRpc gerekebilir,
                // şimdilik lokal bırakıyorum.
            }
            
            // 2. Kamerayı Kendimize Odakla
            if (CameraController.Instance != null)
            {
                CameraController.Instance.SetPlayerController(this);
            }
        }

        private void SetCamera(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            if (!IsOwner) return;
            
            if (CameraController.Instance && sceneName == NetworkBootstrap.Instance.GameSceneName)
            {
                CameraController.Instance.SetPlayerController(this);
            }
        }

        private void ApplyCharacterModel(int index)
        {
            if (_models == null) return;
            
            for (int i = 0; i < _models.Length; i++)
            {
                if(_models[i]) _models[i].SetActive(i == index);
            }
        }
    }
}