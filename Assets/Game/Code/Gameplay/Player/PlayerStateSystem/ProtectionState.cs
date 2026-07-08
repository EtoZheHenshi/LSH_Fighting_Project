using System;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class ProtectionState : PlayerBaseState
    {
        private readonly Action _activeAction;
        
        protected override Action ActiveAction => _activeAction;
        
        public ProtectionState(PlayerController player, StateMachine machine) : base(player, machine)
        {
            _activeAction = Protect;
        }

        private void Protect()
        {
            Debug.Log("Protect");
            //логика защиты
            Machine.ChangeState(Player.GhostState);
        }
    }
}