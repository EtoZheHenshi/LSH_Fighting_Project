using Code.Gameplay.Player.PlayerStateSystem.Base;

namespace Code.Gameplay.Player.PlayerStateSystem.SuperStates
{
    /// <summary>
    /// СУПЕРСОСТОЯНИЕ: общий предок всех состояний "игрок стоит на земле".
    /// 
    /// Ключевая идея иерархической машины состояний (HSM):
    /// если несколько состояний имеют ОДИНАКОВЫЕ переходы или ОДИНАКОВОЕ
    /// поведение — эта общая часть выносится в их общего предка.
    /// 
    /// Что общего у Idle и Move?
    ///   1) из обоих можно прыгнуть по кнопке;
    ///   2) из обоих можно упасть, сойдя с края платформы.
    /// Без суперсостояния эти две проверки пришлось бы копировать
    /// в каждое наземное состояние. С ним — они написаны один раз.
    /// 
    /// Класс abstract: машина никогда не находится "в GroundedState вообще" —
    /// только в одном из его конкретных наследников.
    /// </summary>
    public abstract class GroundedState : PlayerBaseState
    {
        protected GroundedState(PlayerController p, StateMachine m) : base(p, m) { }

        /// <summary>
        /// Общие переходы для ВСЕХ наземных состояний.
        /// 
        /// Возвращает true, если переход произошёл. Наследник обязан
        /// вызвать этот метод В НАЧАЛЕ своего Tick() и, если получил true,
        /// немедленно выйти:
        /// 
        ///     if (TryCommonTransitions()) return;
        /// 
        /// Почему bool, а не просто void? После смены состояния текущий
        /// объект уже неактивен, и продолжать выполнять его Tick() —
        /// ошибка. Возвращаемое значение — явный сигнал наследнику:
        /// "мы уже ушли из этого состояния, останови выполнение".
        /// </summary>
        protected bool TryCommonTransitions()
        {
            // Приоритет 1: прыжок доступен из любого наземного состояния.
            if (Player.Input.Jump == true)
            {
                Machine.ChangeState(Player.JumpState);
                return true;
            }
            //
            // // Приоритет 2: сошли с края платформы -> падаем.
            // // Заметьте: это падение БЕЗ прыжка. Именно ради таких случаев
            // // FallState существует отдельно от JumpState.
            // if (Player.IsGrounded == false)
            // {
            //     Machine.ChangeState(Player.FallState);
            //     return true;
            // }

            return false; // остаёмся в текущем состоянии
        }
    }
}