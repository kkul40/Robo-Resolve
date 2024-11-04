using UnityEngine;

namespace _Project.RGScripts.Player
{
    public class MoveState : PlayerState
    {
        public MoveState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            _player.SetAnimation(PlayerStateType.Move);
        }

        public override void FrameUpdate()
        {
            _player.HandleHorizontalMovement(_settings.AccelerationSpeed, _settings.DecelerationSpeed, _input.MovementInput);

            // var absolute = Mathf.Abs(_player.MoveVelocity.x);
            // _player.SetAnimationSpeed(Mathf.InverseLerp(-_settings.RunSpeed,_settings.RunSpeed, absolute));
            
            if (_input.JumpPressed)
            {
                _stateMachine.ChangeState(PlayerStateType.Jump);
            } 
            else if (Mathf.Abs(_input.MovementInput.x) == 0)
            {
                _stateMachine.ChangeState(PlayerStateType.Idle);
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

        public override bool CanTransitionInto()
        {
            return true;
        }
    }
}