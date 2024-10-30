using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.RGScripts.Player
{
    [CreateAssetMenu(fileName = "Player Config", menuName = "Player", order = 0)]
    public class PlayerConfig : ScriptableObject
    {
        public float DefaultGravityScale = 6;
        public float FallingGravityScale = 8.5f;
        public float RayDetectionLenght = 0.1f;
        public LayerMask WhatIsGround;

        public float AccelerationSpeed = 5;
        public float DecelerationSpeed = 10;
        public float RunSpeed = 5;
        public float JumpForce = 10;
    }
}