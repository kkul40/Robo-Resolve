using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.RGScripts.Player
{
    [CreateAssetMenu(fileName = "Player Config", menuName = "Player", order = 0)]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Player Configs")]
        public float DefaultGravityScale = 6;
        public float FallingGravityScale = 8.5f;
        public float RayDetectionLenght = 0.1f;
        public float DontHugWallAfterWallJumpDelay = 0.4f;
        public float CanJumpAfterUnGroundedDelay = 0.2f;
        public LayerMask WhatIsGround;
        
        [Header("Player Settings")]
        public float AccelerationSpeed = 10;
        public float DecelerationSpeed = 10;
        
        public float AirAccelerationSpeed = 5;
        public float AirDecerationSpeed = 5;
        
        public float RunSpeed = 5;
        public float JumpForce = 20;
        public float WallSlideSpeed = 1;
        public float MaxEnergy = 100;
    }
}