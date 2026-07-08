using System.Collections;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Gameplay.Player.PlayerStateSystem.SuperStates;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class StunState : GroundedState
    {
        private const float Duration = 0.8f;
        private bool _canMove;
        
        public StunState(PlayerController player, StateMachine machine) : base(player, machine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Entered StunState");
            base.Enter();
            
            Player.StartCoroutine(StunDuration());
        }

        public override void Tick()
        {
            if (!_canMove) return;

            if (TryCommonTransitions())
            {
                return;
            }
            
            Machine.ChangeState(Player.IdleState);
        }

        private IEnumerator StunDuration()
        {
            _canMove = false;
            yield return new WaitForSeconds(Duration);
            _canMove = true;
        }
    }
}