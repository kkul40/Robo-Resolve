namespace _Project.RGScripts.Player
{
    public class DoubleJumpState : PlayerState
    {
        public DoubleJumpState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            _player.SetAnimation(PlayerStateType.DoubleJump);
            DoubleJump();
        }

        public override void FrameUpdate()
        {
            _player.HandleHorizontalMovement(_settings.AirAccelerationSpeed, _settings.AirDecerationSpeed, _input.MovementInput);

            if (_player.Velocity.y <= 0)
            {
                _stateMachine.ChangeState(PlayerStateType.Fall);
            }
        }

        private void DoubleJump()
        {
            JumpState jumpState = new JumpState(_player, _settings);
            jumpState.Jump();
            _player.HasDoubleJumped = true;
        }

        public override bool CanTransitionInto()
        {
            return true;
        }
    }
}