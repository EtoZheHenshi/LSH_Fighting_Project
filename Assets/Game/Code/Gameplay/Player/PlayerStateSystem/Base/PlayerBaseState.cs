using System;
using Code.Infrastructure.EventBusSystem;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem.Base
{
    /// Общий предок всех состояний игрока.
    /// 
    /// Зачем он нужен, если уже есть интерфейс IState?
    ///   Интерфейс — это контракт для машины ("что состояние умеет").
    ///   Абстрактный класс — способ не дублировать общий код:
    ///   каждому состоянию нужны одни и те же ссылки на игрока и машину,
    ///   и одинаковый конструктор. Пишем это один раз здесь.
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
            Player.Input.AttackAction = ActiveAction;
        }

        public virtual void Exit()
        {
            Player.Input.AttackAction = null;
        }

        // Tick — abstract: логика кадра обязана быть у КАЖДОГО состояния.
        public virtual void Tick()
        {
            // if (Player.Input.ActiveAction)
            // {
            //     ActiveAction?.Invoke();
            // }
        }
    }
}