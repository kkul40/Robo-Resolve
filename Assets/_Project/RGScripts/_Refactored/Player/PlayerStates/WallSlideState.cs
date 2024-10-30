using UnityEngine;

namespace _Project.RGScripts.Player
{
    public class WallSlideState : PlayerState
    {
        public WallSlideState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            _player.SetAnimation(PlayerStateType.WallSlide);
            _player.SetGravity(0);
        }

        public override void FrameUpdate()
        {
            _player.HandleVerticleMovement(_settings.AccelerationSpeed / 2, _settings.DecelerationSpeed / 2, new Vector2(_input.MovementInput.x, - _settings.WallSlideSpeed));

            if (!_player.IsWalled())
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