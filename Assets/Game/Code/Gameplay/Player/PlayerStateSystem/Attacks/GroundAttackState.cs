using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Gameplay.Player.PlayerStateSystem.SuperStates;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem.Attacks
{
    public sealed class GroundAttackState : GroundedState
    {
        private readonly AttackConfig _attackConfig;
        private readonly LayerMask _enemyMask;

        public GroundAttackState(PlayerController p, StateMachine m, 
            AttackConfig attackConfig, LayerMask enemyMask) : base(p, m)
        {
            _attackConfig = attackConfig;
            _enemyMask = enemyMask;
        }

        public override void Enter()
        {
            base.Enter();
            
            Debug.Log("Entered GroundAttackState");
            
            if (Physics2D.OverlapBox(
                    _attackConfig.transform.position,
                    _attackConfig.AttackSize,
                    0f,
                    _enemyMask))
            {
                Player.Enemy.TakeDamage(_attackConfig.Damage);
            }
            
            Machine.ChangeState(Player.IdleState);
        }

        public override void Tick()
        {
        }
    }
}