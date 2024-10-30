using UnityEngine;

namespace _Project.RGScripts.Player
{
    public class FallState : PlayerState
    {
        private float fallTime = 0;
        
        public FallState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            _player.SetAnimation(PlayerStateType.Fall);
            _player.SetGravity(0);
            fallTime = 0;
        }

        public override void FrameUpdate()
        {
            _player.HandleHorizontalMovement(_settings.AirAccelerationSpeed, _settings.AirDecerationSpeed, _input.MovementInput);

            fallTime += Time.deltaTime;
            
            
            if (fallTime >= 0.15f)
                _player.SetGravity(_settings.FallingGravityScale);
            
            
            if (_player.IsGrounded())
            {
                if (fallTime > 0.6)
                {
                    _stateMachine.ChangeState(PlayerStateType.Land);
                }
                else
                {
                    _stateMachine.ChangeState(PlayerStateType.Idle);
                }
                
                _player.HasDoubleJumped = false;
            }
            else if (_input.JumpPressed && !_player.HasDoubleJumped)
            {
                _stateMachine.ChangeState(PlayerStateType.DoubleJump);
            }
            else if (_player.IsWalled())
            {
                _stateMachine.ChangeState(PlayerStateType.WallSlide);
            }
        }

        public override void Exit()
        {
            fallTime = 0;
            _player.SetGravity(_settings.DefaultGravityScale);
        }

        public override bool CanTransitionInto()
        {
            return true;
        }
    }
}