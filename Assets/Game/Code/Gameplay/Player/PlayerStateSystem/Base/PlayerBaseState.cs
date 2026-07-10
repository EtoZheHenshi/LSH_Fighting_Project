using System;
using Code.Infrastructure.EventBusSystem;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem.Base
{
    public abstract class PlayerBaseState : IState
    {
        protected readonly PlayerController Player;
        protected readonly StateMachine Machine;
        protected readonly EventBusService EventBus;
        
        protected abstract Action ActiveAction { get; }

        protected PlayerBaseState(PlayerController player, StateMachine machine, EventBusService eventBus)
        {
            Player = player;
            Machine = machine;
            EventBus = eventBus;
        }

        public virtual void Enter()
        {
            Player.Input.InputActiveAction = ActiveAction;
        }

        public virtual void Exit()
        {
            Player.Input.InputActiveAction = null;
        }
        
        public virtual void Tick()
        {
        }
    }
}