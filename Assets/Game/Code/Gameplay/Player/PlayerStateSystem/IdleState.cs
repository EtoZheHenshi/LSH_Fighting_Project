using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Gameplay.Player.PlayerStateSystem.SuperStates;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    /// <summary>
    /// Покой. Наследуется от GroundedState, поэтому "бесплатно" умеет:
    /// прыгать по кнопке и падать при сходе с платформы.
    /// Собственной логики осталось совсем мало — в этом выгода иерархии.
    /// </summary>
    public class IdleState : GroundedState
    {
        public IdleState(PlayerController p, StateMachine m) : base(p, m) { }

        public override void Enter()
        {
            Debug.Log("Entered IdleState");
            // player.Animator.Play("Idle");
        }

        public override void Tick()
        {
            // Сначала общие переходы суперсостояния (прыжок, падение).
            // Если один из них сработал — это состояние уже неактивно, выходим.
            if (TryCommonTransitions())
            {
                return;
            }

            // Затем собственный переход состояния: появился ввод -> бежим.
            // if (Mathf.Abs(Player.Input.Horizontal) > 0.01f)
            // {
            //     Machine.ChangeState(Player.MoveState);
            // }
            if (Mathf.Abs(Player.Input.Move) > 0.01f)
            {
                Machine.ChangeState(Player.MoveState);
            }

        }
    }
}