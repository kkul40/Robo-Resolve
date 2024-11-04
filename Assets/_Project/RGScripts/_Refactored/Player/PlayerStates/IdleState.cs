using UnityEngine;

namespace _Project.RGScripts.Player
{
    public class IdleState : PlayerState
    {
        private Vector2 defaultPosition = Vector2.zero; // Use it stop sliding on slopes
        public IdleState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            _player.SetAnimation(PlayerStateType.Idle);
            _player.SetGravity(_settings.DefaultGravityScale);
            _player.CanDash = true;
        }

        public override void FrameUpdate()
        {
            _player.HandleHorizontalMovement(_settings.AccelerationSpeed, _settings.DecelerationSpeed, Vector2.zero);

            if (Mathf.Abs(_input.MovementInput.x) > 0.1f)
            {
                _stateMachine.ChangeState(PlayerStateType.Move);
            }
            else if (_input.JumpPressed)
            {
                _stateMachine.ChangeState(PlayerStateType.Jump);
            }
            else if (_input.DashPressed && _player.CanDash)
            {
                _stateMachine.ChangeState(PlayerStateType.Dash);
            }
            else if (!_player.IsGrounded())
            {
                _stateMachine.ChangeState(PlayerStateType.Fall);
            }
        }

        public override void Exit()
        {
            _player.SetGravity(_settings.DefaultGravityScale);
        }

        public override bool CanTransitionInto()
        {
            return true;
        }
    }
}