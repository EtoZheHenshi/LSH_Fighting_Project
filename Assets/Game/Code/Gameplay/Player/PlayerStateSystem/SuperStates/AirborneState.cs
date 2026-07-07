using Code.Gameplay.Player;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem.SuperStates
{
    /// <summary>
    /// СУПЕРСОСТОЯНИЕ: общий предок всех состояний "игрок в воздухе".
    /// 
    /// Что общего у Jump и Fall? Управление в воздухе (air control):
    /// игрок должен двигаться по горизонтали и во время взлёта,
    /// и во время падения. Пишем это ОДИН раз здесь.
    /// 
    /// Обратите внимание на отличие от GroundedState:
    ///   GroundedState выносит общие ПЕРЕХОДЫ  -> метод TryCommonTransitions,
    ///   AirborneState выносит общее ПОВЕДЕНИЕ -> виртуальный Tick.
    /// Это два основных приёма HSM (иерархической машины состояний), и здесь мы демонстрируем оба.
    /// </summary>
    // todo
    public abstract class AirborneState : PlayerBaseState
    {
        protected AirborneState(PlayerController p, StateMachine m) : base(p, m)
        {
        }

        /// <summary>
        /// Общее поведение каждый кадр в воздухе: air control.
        /// 
        /// Наследник переопределяет Tick() и первым делом вызывает
        /// base.Tick() — тем самым он "наследует" движение в воздухе,
        /// а затем добавляет свои проверки переходов.
        /// 
        /// Здесь безопасно использовать base.Tick() (а не bool-метод),
        /// потому что этот код НЕ совершает переходов — он только
        /// двигает игрока. Правило такое:
        ///   общая логика меняет состояние      -> bool-метод + return,
        ///   общая логика только "делает работу" -> base.Tick().
        /// </summary>
        public override void Tick()
        {
            // Горизонталь управляется игроком, вертикаль — гравитацией.
            Player.transform.position =
                new Vector2(Player.transform.position.x + Player.Input.Move * Player.MoveSpeed * Time.deltaTime,
                    Player.transform.position.y);
        }

        /// <summary>
        /// Общий для всех воздушных состояний выбор, куда приземляться.
        /// Смотрим на ТЕКУЩИЙ ввод: если игрок держит движение —
        /// сразу в Move, без "залипания" в Idle на один кадр.
        /// </summary>
        protected void LandTo()
        {
            if (Mathf.Abs(Player.Input.Move) > 0.01f)
            {
                Machine.ChangeState(Player.MoveState);
            }
            else Machine.ChangeState(Player.IdleState);
        }
    }
}