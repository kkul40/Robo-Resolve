using UnityEngine;

namespace _Project.RGScripts.Player
{
    public class DashState : PlayerState
    {
        public DashState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            Dash();
        }

        public override void FrameUpdate()
        {
            _player.HandleHorizontalMovement(_settings.AirAccelerationSpeed, _settings.AirDecerationSpeed, _input.MovementInput);

            if (_player.Velocity.y <= 0)
            {
                _stateMachine.ChangeState(PlayerStateType.Fall);
            }
        }

        private void Dash()
        {
            //DashState dashState = new DashState(_player, _settings);
            //_player.SetVelocity(_player.Velocity.x, 0);

            Vector2 direction = Vector2.left;
            if(_player.IsFacingRight)
                direction = Vector2.right;
            _rigidbody2D.AddForce(direction.normalized * _settings.DashForce, ForceMode2D.Impulse);
        }

        public override bool CanTransitionInto()
        {
            return true;
        }
    }
}