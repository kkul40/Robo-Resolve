using System.Collections.Generic;
using UnityEngine;

namespace _Project.RGScripts.Player
{
    public enum PlayerStateType
    {
        Idle,
        Duck,
        Move,
        Jump,
        DoubleJump,
        Fall,
        Land,
        WallSlide,
        WallJump,
    }
    
    [RequireComponent(typeof(Player))]
    public class PlayerStateMachine : MonoBehaviour
    {
        private Player _player;
        private SpriteRenderer _spriteRenderer;
        private Dictionary<PlayerStateType, PlayerState> _playerStates;
        
        [SerializeField] private PlayerStateType _currentState;
        public PlayerStateType PreviousState;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _playerStates = new Dictionary<PlayerStateType, PlayerState>
            {
                [PlayerStateType.Idle] = new IdleState(_player, _player._playerConfig),
                [PlayerStateType.Move] = new MoveState(_player, _player._playerConfig),
                [PlayerStateType.Jump] = new JumpState(_player, _player._playerConfig),
                [PlayerStateType.DoubleJump] = new DoubleJumpState(_player, _player._playerConfig),
                [PlayerStateType.Fall] = new FallState(_player, _player._playerConfig),
                [PlayerStateType.Land] = new LandState(_player, _player._playerConfig),
                [PlayerStateType.WallSlide] = new WallSlideState(_player, _player._playerConfig),
                [PlayerStateType.WallJump] = new WallJumpState(_player, _player._playerConfig),
            };
            
            ChangeState(PlayerStateType.Idle);
        }

        private void Update()
        {
            _playerStates[_currentState].FrameUpdate();
        }

        private void FixedUpdate()
        {
            _playerStates[_currentState].FixedFrameUpdate();
        }

        public void ChangeState(PlayerStateType nextState)
        {
            PreviousState = _currentState;
            
            _playerStates[_currentState].Exit();
            _currentState = nextState;
            _playerStates[_currentState].Enter();
        }
    }
}