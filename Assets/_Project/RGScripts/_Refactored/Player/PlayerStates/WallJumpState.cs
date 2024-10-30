using UnityEngine;

namespace _Project.RGScripts.Player
{
    public class WallJumpState : PlayerState
    {
        private float timer = 0;
        public WallJumpState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            WallJump();
            _player.SetAnimation(PlayerStateType.Jump);
            _player.FlipPlayer(!_player.IsFacingRight);

            timer = 0;
        }

        public override void FrameUpdate()
        {
            _player.HandleHorizontalMovement(_settings.AirAccelerationSpeed, _settings.AirDecerationSpeed, _input.MovementInput);

            timer += Time.deltaTime;
            
            if (_player.IsWalled() && timer >= _settings.DontHugWallAfterWallJumpDelay)
            {
                _stateMachine.ChangeState(PlayerStateType.WallSlide);
            }
            else if (_player.Velocity.y <= 0)
            {
                _stateMachine.ChangeState(PlayerStateType.Fall);
            }
        }

        private void WallJump()
        {
            _player.SetVelocity(0, 0);
            Vector2 jumpDirection = Vector2.zero;

            if (_player.IsFacingRight)
            {
                if (_input.MovementInput.x > 0)
                {
                    jumpDirection.x = -0.3f;
                }
                else
                {
                    jumpDirection.x = -1;
                }
            }
            else
            {
                if (_input.MovementInput.x < 0)
                {
                    jumpDirection.x = 0.3f;
                }
                else
                {
                    jumpDirection.x = 1;
                }
            }
            
            jumpDirection.y = 1;
            
            _rigidbody2D.AddForce(jumpDirection * _settings.JumpForce, ForceMode2D.Impulse);
        }

        public override bool CanTransitionInto()
        {
            return true;
        }
    }
}