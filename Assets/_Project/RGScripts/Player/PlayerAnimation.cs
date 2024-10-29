using System;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.RGScripts.Player
{
    public enum PlayerStates
    {
        Idle,
        Walk,
        Jump,
        DoubleJump,
        Fall,
        Land,
    }
    
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator _animator;

        private string currentState = "";

        private const string PLAYER_IDLE = "IDLE";
        private const string PLAYER_WALK = "WALKING";
        private const string PLAYER_JUMP = "JUMP";
        private const string PLAYER_DOUBLEJUMP = "DOUBLEJUMP";
        private const string PLAYER_FALL = "FALL";
        private const string PLAYER_LAND = "LAND";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void PlayAnimation(PlayerStates playerStates)
        {
            if (currentState == playerStates.ToString()) return;

            switch (playerStates)
            {
                case PlayerStates.Idle:
                    _animator.Play(PLAYER_IDLE);
                    break;
                case PlayerStates.Walk:
                    _animator.Play(PLAYER_WALK);
                    break;
                case PlayerStates.Jump:
                    _animator.Play(PLAYER_JUMP);
                    break;
                case PlayerStates.DoubleJump:
                    _animator.Play(PLAYER_DOUBLEJUMP);
                    break;
                case PlayerStates.Fall:
                    _animator.Play(PLAYER_FALL);
                    break;
                case PlayerStates.Land:
                    _animator.Play(PLAYER_LAND);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerStates), playerStates, null);
            }
        }

       
    }
}