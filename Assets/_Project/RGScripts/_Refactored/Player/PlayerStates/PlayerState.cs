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
}