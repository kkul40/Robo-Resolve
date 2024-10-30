using System;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.RGScripts.Player
{
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

        public void PlayAnimation(PlayerStateType playerStates)
        {
            if (currentState == playerStates.ToString()) return;

            switch (playerStates)
            {
                case PlayerStateType.Idle:
                    _animator.Play(PLAYER_IDLE);
                    break;
                case PlayerStateType.Move:
                    _animator.Play(PLAYER_WALK);
                    break;
                case PlayerStateType.Jump:
                    _animator.Play(PLAYER_JUMP);
                    break;
                case PlayerStateType.DoubleJump:
                    _animator.Play(PLAYER_DOUBLEJUMP);
                    break;
                case PlayerStateType.Fall:
                    _animator.Play(PLAYER_FALL);    
                    break;
                case PlayerStateType.Land:
                    _animator.Play(PLAYER_LAND);
                    break;
                // default:
                // {
                //     Debug.LogWarning(playerStates.ToString() + " : Is Missing in Animation");
                //     // throw new ArgumentOutOfRangeException(nameof(playerStates), playerStates, null);
                //     break;
                // }
            }
        }
        public void SetAnimationSpeed(float speed)
        {
            _animator.speed = speed;
        }
    }
}