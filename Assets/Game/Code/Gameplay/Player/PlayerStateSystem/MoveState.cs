using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Gameplay.Player.PlayerStateSystem.SuperStates;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    /// <summary>
    /// Бег по земле. Как и Idle, наследует общие переходы от GroundedState.
    /// Сравните с версией без иерархии: там проверка прыжка дублировалась
    /// и в Idle, и в Move. Теперь она в одном месте — в суперсостоянии.
    /// </summary>
    public class MoveState : GroundedState
    {
        public MoveState(PlayerController p, StateMachine m) : base(p, m) { }

        public override void Enter()
        {
            Debug.Log("Entered MoveState");
            // player.Animator.Play("Run");
        }

        public override void Tick()
        {
            // Общие переходы — всегда первыми.
            if (TryCommonTransitions())
            {
                return;
            }
            
            // Собственный переход: ввод пропал -> покой.
            if (Mathf.Abs(Player.Input.Move) < 0.01f)
            {
                Machine.ChangeState(Player.IdleState);
            }
            
            // Собственное поведение: движение по земле.
            Player.transform.position = new Vector2(
                Player.transform.position.x + Player.Input.Move * Player.MoveSpeed * Time.deltaTime,
                Player.transform.position.y);
        }
    }
}