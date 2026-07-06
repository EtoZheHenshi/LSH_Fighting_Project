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
        // protected — поля видны наследникам, но закрыты для всех остальных.
        // readonly — ссылки задаются один раз в конструкторе и не меняются.
        protected readonly PlayerController Player;
        protected readonly StateMachine Machine;

        protected PlayerBaseState(PlayerController player, StateMachine machine)
        {
            this.Player = player;
            this.Machine = machine;
        }

        // Enter и Exit — virtual с пустым телом: состояние переопределяет
        // их только если ему действительно нужно что-то сделать на входе/выходе.
        public virtual void Enter() { }
        public virtual void Exit() { }

        // Tick — abstract: логика кадра обязана быть у КАЖДОГО состояния.
        public abstract void Tick();
    }
}