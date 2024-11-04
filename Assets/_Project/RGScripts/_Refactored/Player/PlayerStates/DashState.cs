using UnityEngine;

namespace _Project.RGScripts.Player
{
    public class DashState : PlayerState
    {
        private float dashTimer = 0;
        public DashState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            _player.SetAnimation(PlayerStateType.Dash);
            _player.SetVelocity(new Vector2(_player.Velocity.x, 0));
            _player.SetGravity(0);
            _player.CanDash = false;
            dashTimer = 0;
            Dash();
        }

        public override void FrameUpdate()
        {
            _player.HandleHorizontalMovement(_settings.AirAccelerationSpeed, _settings.DecelerationSpeed, new Vector2(_input.MovementInput.x, 0));

            dashTimer += Time.deltaTime;

            if (dashTimer >= _settings.DashForce / 100) 
            {
                if (_player.IsGrounded())
                {
                    _stateMachine.ChangeState(PlayerStateType.Idle);
                }
                else
                {
                    _stateMachine.ChangeState(PlayerStateType.Fall);
                }
            }
        }

        private void Dash()
        {
            Vector2 direction = Vector2.left;
            if(_player.IsFacingRight)
                direction = Vector2.right;

            if (!_player.IsGrounded())
                direction.y = _input.MovementInput.y > 0 ? 1 : 0;
            
            _rigidbody2D.AddForce(direction.normalized * _settings.DashForce, ForceMode2D.Impulse);
        }

        public override void Exit()
        {
            _player.SetVelocity(new Vector2(_player.Velocity.x, 0));
            _player.SetGravity(_settings.DefaultGravityScale);
        }

        public override bool CanTransitionInto()
        {
            return true;
        }
    }
}