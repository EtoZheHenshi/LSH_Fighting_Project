using System;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class GhostState : PlayerBaseState
    {
        private readonly Action _activeAction;
        
        protected override Action ActiveAction => _activeAction;
        
        public GhostState(PlayerController player, StateMachine machine) : base(player, machine)
        {
            _activeAction = Possession;
        }

        private void Possession()
        {
            Debug.Log("Possession");
            //логика завладевания телом
            Machine.ChangeState(Player.AttackState);
        }
    }
}