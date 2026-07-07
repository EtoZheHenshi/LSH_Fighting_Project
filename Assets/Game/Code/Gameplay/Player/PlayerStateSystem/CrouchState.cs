using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Gameplay.Player.PlayerStateSystem.SuperStates;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class CrouchState : GroundedState
    {
        public CrouchState(PlayerController p, StateMachine m) : base(p, m)
        {
        }

        private Vector3 _oldScale;

        public override void Enter()
        {
            Debug.Log("Entered CrouchState");
            _oldScale = Player.transform.localScale;
            Player.transform.localScale = new Vector3(Player.transform.localScale.x,
                Player.transform.localScale.y * 0.5f, Player.transform.localScale.z);
        }

        public override void Tick()
        {
            if (TryCommonTransitions())
            {
                return;
            }

            if (!Player.Input.Crouch)
            {
                Machine.ChangeState(Player.IdleState);
            }
        }

        public override void Exit()
        {
            Player.transform.localScale = _oldScale;
        }
    }
}