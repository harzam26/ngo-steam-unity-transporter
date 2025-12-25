namespace Player.States
{
    public abstract class PlayerBaseState
    {
        protected PlayerStateManager _playerStateManager;
        protected PlayerBaseState(PlayerStateManager context)
        {
            _playerStateManager = context;
        }

        public virtual void Enter() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void Exit() { }
    }
}