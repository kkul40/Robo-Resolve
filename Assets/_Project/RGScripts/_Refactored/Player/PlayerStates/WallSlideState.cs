using UnityEngine;

namespace _Project.RGScripts.Player
{
    public class WallSlideState : PlayerState
    {
        private bool isFacingRight;
        public WallSlideState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            _player.SetAnimation(PlayerStateType.WallSlide);
            _player.SetVelocity(Vector2.zero);
            _player.SetGravity(0);
            isFacingRight = _player.IsFacingRight;
        }

        public override void FrameUpdate()
        {
            _player.HandleVerticleMovement(1, 1, new Vector2(_input.MovementInput.x, - _settings.WallSlideSpeed));
            
            if (_input.JumpPressed)
            {
                _stateMachine.ChangeState(PlayerStateType.WallJump);
            }
            else if (!_player.IsWalled())
            {
                _stateMachine.ChangeState(PlayerStateType.Fall);
            }
            else if (_player.IsGrounded())
            {
                _stateMachine.ChangeState(PlayerStateType.Idle);
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