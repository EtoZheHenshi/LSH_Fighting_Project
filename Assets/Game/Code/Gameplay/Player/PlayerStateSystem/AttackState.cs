using System;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class AttackState : PlayerBaseState
    {
        private readonly Action _activeAction;
        
        protected override Action ActiveAction => _activeAction;
        
        public AttackState(PlayerController player, StateMachine machine) : base(player, machine)
        {
            _activeAction = Attack;
        }

        private void Attack()
        {
            Debug.Log("Attack");
            //логика атаки
            Machine.ChangeState(Player.ProtectionState);
        }
    }
}