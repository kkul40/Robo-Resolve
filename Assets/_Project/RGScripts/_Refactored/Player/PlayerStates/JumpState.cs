using UnityEngine;

namespace _Project.RGScripts.Player
{
    public class JumpState : PlayerState
    {
        public JumpState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            Jump();
            _player.SetAnimation(PlayerStateType.Jump);
        }

        public override void FrameUpdate()
        {
            _player.HandleHorizontalMovement(_settings.AirAccelerationSpeed, _settings.AirDecerationSpeed, _input.MovementInput);

            if (_player.Velocity.y <= 0)
            {
                _stateMachine.ChangeState(PlayerStateType.Fall);
            }
            else if (_input.JumpPressed && !_player.HasDoubleJumped)
            {
                _stateMachine.ChangeState(PlayerStateType.DoubleJump);
            }
        }

        public override bool CanTransitionInto()
        {
            return true;
        }
        
        public void Jump()
        {
            _player.SetVelocity(_player.Velocity.x, 0);
            _rigidbody2D.AddForce(Vector2.up * _settings.JumpForce, ForceMode2D.Impulse);
        }
    }
}