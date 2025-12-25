using UnityEngine;

namespace Player.States
{
    public class PlayerNormalState : PlayerBaseState
    {
        public PlayerNormalState(PlayerStateManager context) : base(context) { }

        public override void Enter()
        {
        }

        public override void Update()
        {
            // Sadece Owner hareket edebilir
            if (_playerStateManager.IsOwner)
            {
                Vector2 input = PlayerInputBridge.Instance.MoveInput;
                _playerStateManager.Locomotion.SetInput(input);
            }
        }
    }
}