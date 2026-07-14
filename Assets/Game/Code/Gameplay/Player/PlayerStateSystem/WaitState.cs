using System;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class WaitState : PlayerBaseState
    {
        protected override Action ActiveAction => () => {};

        public WaitState(PlayerController player, StateMachine machine, EventBusService eventBus) 
            : base(player, machine, eventBus)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            Player.PlayerIcons.SetRoleIcon(true);
        }

        public override void Exit()
        {
            base.Exit();
            
            Player.PlayerIcons.SetRoleIcon(false);
        }
    }
}