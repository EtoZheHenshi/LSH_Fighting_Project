using System;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class WaitState : PlayerBaseState
    {
        protected override Action ActiveAction => () => {};

        public WaitState(PlayerController player, StateMachine machine, EventBusService eventBus) 
            : base(player, machine, eventBus)
        {
        }
    }
}