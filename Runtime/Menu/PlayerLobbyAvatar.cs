using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Menu
{
    public class PlayerLobbyAvatar : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _nameTag;
        [SerializeField] private GameObject[] _characterModels;

        private readonly NetworkVariable<int> _characterIndex =
            new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        
        private readonly NetworkVariable<FixedString64Bytes> _playerName = 
            new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                // GÜVENLİ İSİM ALMA MANTIĞI
                string myName = $"Player {OwnerClientId}"; // Varsayılan değer
        
                if (SteamManager.Instance != null && !string.IsNullOrEmpty(SteamManager.Instance.MyName))
                {
                    myName = SteamManager.Instance.MyName;
                }

                // FixedString64Bytes maximum 64 byte (utf8) alır. İsmi kısaltmakta fayda var.
                if (myName.Length > 60) myName = myName.Substring(0, 60);

                UpdateNameServerRpc(myName);
            }
    
            _playerName.OnValueChanged += (_, newName) => UpdateName(newName.ToString());
    
            if(!string.IsNullOrEmpty(_playerName.Value.ToString()))
                UpdateName(_playerName.Value.ToString());

            ApplyCharacterModel(_characterIndex.Value);
        }

        [ServerRpc]
        private void UpdateNameServerRpc(string playerName)
        {
            // Gelen string null ise boş string'e çevir
            string safeName = playerName ?? ""; 
            _playerName.Value = safeName;
        }
        
        public void SetCharacterIndexServer(int index)
        {
            if (IsServer)
            {
                _characterIndex.Value = Mathf.Clamp(index, 0, _characterModels.Length - 1);
            }
        }
        
        private void ApplyCharacterModel(int index)
        {
            for (int i = 0; i < _characterModels.Length; i++)
            {
                _characterModels[i].SetActive(i == index);
            }
        }

        private void UpdateName(string name)
        {
            if (_nameTag)
            {
                _nameTag.text = name;
            }
        }

        private void Update()
        {
            if (Camera.main && _nameTag)
            {
                var t = _nameTag.transform;
                t.rotation = Quaternion.LookRotation(t.position - Camera.main.transform.position);
            }
        }
        
        public int GetCharacterIndex() => _characterIndex.Value;
    }
}