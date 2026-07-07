using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Gameplay.Player.PlayerStateSystem.SuperStates;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    /// <summary>
    /// Взлёт: восходящая часть прыжка.
    /// 
    /// ГЛАВНОЕ, что нужно увидеть студентам в этом классе:
    /// здесь НЕТ кода движения в воздухе. Совсем.
    /// Air control наследуется из AirborneState через base.Tick().
    /// Состояние добавляет только то, что уникально для взлёта:
    ///   - импульс вверх при входе (Enter),
    ///   - переход в падение на пике дуги (Tick).
    /// </summary>
    public class JumpState : AirborneState
    {
        public JumpState(PlayerController p, StateMachine m) : base(p, m) { }

        public override void Enter()
        {
            Debug.Log("Entered JumpState");
            // player.Animator.Play("Jump");

            // Импульс — разовое действие, поэтому Enter, а не Tick.
            // Player.Body.linearVelocity = new Vector2(
            //     Player.Body.linearVelocity.x,
            //     Player.JumpForce);
        }

        public override void Tick()
        {
            // Наследуем поведение суперсостояния: air control.
            base.Tick();

            // Уникальный переход взлёта: вертикальная скорость иссякла —
            // мы на пике дуги, дальше физика тянет вниз. Переходим в Fall.
            // Заметьте: этот переход вызван ФИЗИКОЙ, а не кнопкой.
            // Состояния могут реагировать на разные источники событий.
            // if (Player.Body.linearVelocity.y <= 0f)
            // {
            //     Machine.ChangeState(Player.FallState);
            // }
        }
    }
}