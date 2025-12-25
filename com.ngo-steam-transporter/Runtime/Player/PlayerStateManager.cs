using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Player.States;

namespace Player
{
    public enum PlayerState
    {
        Normal,
    }
    
    [RequireComponent(typeof(PlayerLocomotion))]
    [RequireComponent(typeof(NetworkTransform))]
    public class PlayerStateManager : NetworkBehaviour
    {
        public Rigidbody Rb { get; private set; }
        public NetworkTransform NetTransform { get; private set; }
        public Collider MainCollider { get; private set; }
        private PlayerLocomotion _playerLocomotion;

        public NetworkVariable<PlayerState> CurrentStateEnum = new NetworkVariable<PlayerState>(PlayerState.Normal);
        private PlayerBaseState _currentState;
        
        // State Cache
        private Dictionary<PlayerState, PlayerBaseState> _stateDictionary = new Dictionary<PlayerState, PlayerBaseState>();
        
        public PlayerLocomotion Locomotion => _playerLocomotion;

        private void Awake()
        {
            Rb = GetComponent<Rigidbody>();
            NetTransform = GetComponent<NetworkTransform>();
            MainCollider = GetComponent<Collider>();
            _playerLocomotion = GetComponent<PlayerLocomotion>();

            // State'leri oluştur ve Dictionary'e at
            _stateDictionary.Add(PlayerState.Normal, new PlayerNormalState(this));
        }

        public override void OnNetworkSpawn()
        {
            CurrentStateEnum.OnValueChanged += OnStateEnumChanged;
            
            // Başlangıç state'ini ayarla
            SwitchState(CurrentStateEnum.Value);
        }

        public override void OnNetworkDespawn()
        {
            CurrentStateEnum.OnValueChanged -= OnStateEnumChanged;
        }

        private void Update()
        {
            // State'in Update'ini çalıştır
            _currentState?.Update();
        }

        private void FixedUpdate()
        {
            _currentState?.FixedUpdate();
        }

        // --- STATE DEĞİŞİM MANTIĞI ---

        private void OnStateEnumChanged(PlayerState oldState, PlayerState newState)
        {
            SwitchState(newState);
        }

        public void SetState(PlayerState newState)
        {
            if (!IsServer) return;
            CurrentStateEnum.Value = newState;
        }

        private void SwitchState(PlayerState newState)
        {
            _currentState?.Exit();
            
            if (_stateDictionary.TryGetValue(newState, out var nextState))
            {
                _currentState = nextState;
                _currentState.Enter();
            }
        }

        public NetworkObject GetNetworkObject() => NetworkObject;
    }
}