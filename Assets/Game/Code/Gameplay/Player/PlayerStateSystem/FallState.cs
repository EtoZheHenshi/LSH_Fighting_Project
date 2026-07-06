using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Gameplay.Player.PlayerStateSystem.SuperStates;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    /// <summary>
    /// Падение: нисходящая часть полёта.
    /// Сюда попадают ДВУМЯ путями:
    ///   1) Jump -> Fall (пик прыжка),
    ///   2) Idle/Move -> Fall (сошли с края платформы — общий переход
    ///      из GroundedState).
    /// Один класс обслуживает оба случая — ещё один плюс явных состояний.
    /// 
    /// Движения в воздухе здесь тоже нет — оно наследуется из AirborneState.
    /// </summary>
    public class FallState : AirborneState
    {
        public FallState(PlayerController p, StateMachine m) : base(p, m) { }

        public override void Enter()
        {
            // player.Animator.Play("Fall");
        }

        public override void Tick()
        {
            // Наследуем air control.
            base.Tick();

            // Уникальный переход падения: коснулись земли -> приземляемся.
            // Куда именно (Idle или Move) — решает общий метод суперсостояния.
            // if (Player.IsGrounded)
            // {
            //     LandTo();
            // }
        }
    }
}