using Unity.VisualScripting;
using UnityEngine;

namespace _Project.RGScripts.Player
{
    public abstract class PlayerState
    {
        protected Player _player;
        protected PlayerStateMachine _stateMachine;
        protected PlayerConfig _settings;
        
        protected PlayerInput _input;
        protected SpriteRenderer _spriteRenderer;
        protected Collider2D _groundCollider;
        protected Rigidbody2D _rigidbody2D;

        public PlayerState(Player player, PlayerConfig settings)
        {
            _player = player;
            _stateMachine = player.GetComponent<PlayerStateMachine>();
            _settings = settings;
            
            _input = player.GetComponent<PlayerInput>();
            _spriteRenderer = player.GetComponent<SpriteRenderer>();
            _groundCollider = player.GroundCollider;
            _rigidbody2D = player.GetComponent<Rigidbody2D>();
        }
        
        // -------------------------------
        //          Common Methods
        // -------------------------------
        public virtual void Enter(){}
        public virtual void FrameUpdate(){}
        public virtual void FixedFrameUpdate(){}
        public virtual void Exit(){}
        public abstract bool CanTransitionInto();
        // -------------------------------
    }

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
        }

        public override void FrameUpdate()
        {
            _player.HandleHorizontalMovement(_settings.AccelerationSpeed, _settings.DecelerationSpeed, Vector2.zero);

            Debug.Log(_player.IsGrounded);
            if (Mathf.Abs(_input.MovementInput.x) > 0.1f)
            {
                _stateMachine.ChangeState(PlayerStateType.Move);
            }
            else if (_input.JumpPressed)
            {
                _stateMachine.ChangeState(PlayerStateType.Jump);
            }
            else if (!_player.IsGrounded)
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

            var absolute = Mathf.Abs(_player.MoveVelocity.x);
            _player.SetAnimationSpeed(Mathf.InverseLerp(-_settings.RunSpeed,_settings.RunSpeed, absolute));
            
            if (_input.JumpPressed)
            {
                _stateMachine.ChangeState(PlayerStateType.Jump);
            }
            else if (Mathf.Abs(_player.Velocity.x) < 0.2f)
            {
                _stateMachine.ChangeState(PlayerStateType.Idle);
            }
            else if (!_player.IsGrounded)
            {
                _stateMachine.ChangeState(PlayerStateType.Fall);
            }
        }

        public override bool CanTransitionInto()
        {
            return true;
        }
    }
    
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
            _player.HandleHorizontalMovement(_settings.AccelerationSpeed / 2, _settings.DecelerationSpeed / 2, _input.MovementInput);

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
    
    public class DoubleJumpState : PlayerState
    {
        public DoubleJumpState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            _player.SetAnimation(PlayerStateType.DoubleJump);
            DoubleJump();
        }

        public override void FrameUpdate()
        {
            _player.HandleHorizontalMovement(_settings.AccelerationSpeed / 2, _settings.DecelerationSpeed / 2, _input.MovementInput);

            if (_player.Velocity.y <= 0)
            {
                _stateMachine.ChangeState(PlayerStateType.Fall);
            }
        }

        private void DoubleJump()
        {
            JumpState jumpState = new JumpState(_player, _settings);
            jumpState.Jump();
            _player.HasDoubleJumped = true;
        }

        public override bool CanTransitionInto()
        {
            return true;
        }
    }

    public class FallState : PlayerState
    {
        private float fallTime = 0;
        
        public FallState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            _player.SetAnimation(PlayerStateType.Fall);
            fallTime = 0;
        }

        public override void FrameUpdate()
        {
            _player.HandleHorizontalMovement(_settings.AccelerationSpeed / 2, _settings.DecelerationSpeed / 2, _input.MovementInput);

            fallTime += Time.deltaTime;
            
            if (fallTime < 0.15f)
            {
                _player.SetGravity(0);
            }
            else
            {
                _player.SetGravity(_settings.FallingGravityScale);
            }

            if (_player.IsGrounded)
            {
                if (fallTime > 0.5)
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
    
    public class LandState : PlayerState
    {
        private float landTimer = 0;
        
        public LandState(Player player, PlayerConfig settings) : base(player, settings)
        {
        }

        public override void Enter()
        {
            _player.SetAnimation(PlayerStateType.Land);
            _player.SetVelocity(Vector2.zero);
        }

        public override void FrameUpdate()
        {
            landTimer += Time.deltaTime;
            if (landTimer > 0.5f)
            {
                _stateMachine.ChangeState(PlayerStateType.Idle);
            }
        }

        public override void Exit()
        {
            landTimer = 0;
        }

        public override bool CanTransitionInto()
        {
            return true;
        }
    }
}