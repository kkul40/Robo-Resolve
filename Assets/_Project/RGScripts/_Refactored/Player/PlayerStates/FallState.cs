using UnityEngine;

namespace _Project.RGScripts.Player
{
    public class FallState : PlayerState
    {
        private float fallTime = 0;
        private float inputBufferTimer = 0;
        private bool checkEdge = true;
        private bool isClimbingEdge = false;
        
        public FallState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            _player.SetAnimation(PlayerStateType.Fall);
            _player.SetGravity(0);
            fallTime = 0;
            inputBufferTimer = 0;
            checkEdge = true;
            isClimbingEdge = false;
        }

        public override void FrameUpdate()
        {
            if(!isClimbingEdge)
                _player.HandleHorizontalMovement(_settings.AirAccelerationSpeed, _settings.AirDecerationSpeed, _input.MovementInput);

            fallTime += Time.deltaTime;
            inputBufferTimer += Time.deltaTime;
            
            if (fallTime >= 0.15f)
                _player.SetGravity(_settings.FallingGravityScale);

            if (_input.JumpPressed)
                inputBufferTimer = 0;

            if (checkEdge && _player.IsEdged() && _player.IsWalled())
            {
                isClimbingEdge = true;
            }

            if (isClimbingEdge)
            {
                _player.SetGravity(0);
                _player.SetVelocity(Vector2.zero);

                bool exitClimbing = _player.IsFacingRight ? (_input.MovementInput.x < 0) : (_input.MovementInput.x > 0);
                if (_input.MovementInput.y < 0 || exitClimbing)
                {
                    checkEdge = false;
                    isClimbingEdge = false;
                    _player.SetGravity(_settings.FallingGravityScale);
                }
                else if (_input.MovementInput.y > 0 || _input.JumpPressed)
                {
                    float xPos = _player.IsFacingRight ? 0.5f : -0.5f;
                    _player.SetPosition(_player.Position.x + xPos, _player.Position.y + 0.5f);
                    _stateMachine.ChangeState(PlayerStateType.Idle);
                }
            }
            else if (_player.IsGrounded())
            {
                if (fallTime > 0.6)
                    _stateMachine.ChangeState(PlayerStateType.Land);
                else if(inputBufferTimer <= _settings.CanRightBeforeGroundedDelay)
                    _stateMachine.ChangeState(PlayerStateType.Jump);
                else
                    _stateMachine.ChangeState(PlayerStateType.Idle);
            
                _player.HasDoubleJumped = false;
            }
            else if (_input.JumpPressed && _stateMachine.PreviousState == PlayerStateType.Move && fallTime <= _settings.CanJumpAfterUnGroundedDelay)
                _stateMachine.ChangeState(PlayerStateType.Jump);
            else if (_input.JumpPressed && !_player.HasDoubleJumped)
                _stateMachine.ChangeState(PlayerStateType.DoubleJump);
            else if (_player.IsWalled())
                _stateMachine.ChangeState(PlayerStateType.WallSlide);
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
    
    public class EdgeClimb : PlayerState
    {
        public EdgeClimb(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
        }
        

        public override bool CanTransitionInto()
        {
            return true;
        }
    }
}