using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Gameplay.Player.PlayerStateSystem.SuperStates;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem.Attacks
{
    public sealed class GroundAttackState : GroundedState
    {
        private readonly AttackConfig _attackConfig;
        private readonly LayerMask _enemyMask;

        private Collider2D _enemy;

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

            _enemy = Physics2D.OverlapBox(
                _attackConfig.transform.position,
                _attackConfig.AttackSize,
                0f,
                _enemyMask);
            
            if (_enemy != null)
            {
                _enemy.GetComponent<PlayerController>().TakeDamage(_attackConfig.Damage);
                _enemy = null;
            }
        }

        public override void Tick()
        {
            throw new System.NotImplementedException();
        }
    }
}