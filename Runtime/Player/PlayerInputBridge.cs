using System;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerInputBridge : Singleton<PlayerInputBridge>
    {
        private InputSystem_Actions _inputActions;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsSprinting { get; private set; }
        
        public event Action OnJumpPressed;
        public event Action OnInteractPressed;
        public event Action OnPrimaryActionPressed;
        public event Action OnPrimaryActionReleased;

        public void Start()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
            _inputActions.Player.Enable();

            SubscribeToInputs();
        }

        protected override void OnDestroy()
        {
            UnsubscribeFromInputs();
            _inputActions.Player.Disable();
            _inputActions.Dispose();
        }

        private void SubscribeToInputs()
        {
            // 1. Movement (Vector2)
            _inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

            // 2. Look / Aiming (Vector2)
            _inputActions.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Look.canceled += ctx => LookInput = Vector2.zero;

            // 3. Sprint (Bool) - Hold logic
            _inputActions.Player.Sprint.performed += ctx => IsSprinting = true;
            _inputActions.Player.Sprint.canceled += ctx => IsSprinting = false;

            // 4. One-Shot Actions (Events)
            _inputActions.Player.Jump.performed += ctx => OnJumpPressed?.Invoke();
            _inputActions.Player.Interact.performed += ctx => OnInteractPressed?.Invoke();
            
            // 5. Attack / Throw
            _inputActions.Player.Attack.performed += ctx => OnPrimaryActionPressed?.Invoke();
            _inputActions.Player.Attack.canceled += ctx => OnPrimaryActionReleased?.Invoke();
        }

        private void UnsubscribeFromInputs()
        {
            _inputActions.Player.Move.performed -= ctx => MoveInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Move.canceled -= ctx => MoveInput = Vector2.zero;
        }
    }
}