using UnityEngine;

namespace _Project.RGScripts.Player
{
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