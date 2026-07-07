using System.Collections;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Gameplay.Player.PlayerStateSystem.SuperStates;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem.Attacks
{
    public sealed class GroundAttackState : GroundedState
    {
        private readonly AttackConfig _attackConfig;
        private readonly LayerMask _enemyMask;

        private bool _canAttack;

        public GroundAttackState(PlayerController p, StateMachine m, 
            AttackConfig attackConfig, LayerMask enemyMask) : base(p, m)
        {
            _attackConfig = attackConfig;
            _enemyMask = enemyMask;
            _canAttack = true;
        }

        public override void Enter()
        {
            base.Enter();

            if (_canAttack)
            {
                Player.StartCoroutine(AttackDelay());
                
                Debug.Log("Entered GroundAttackState");

                if (Physics2D.OverlapBox(
                        _attackConfig.transform.position,
                        _attackConfig.AttackSize,
                        0f,
                        _enemyMask))
                {
                    Player.Enemy.TakeDamage(_attackConfig.Damage);
                }

                Player.StartCoroutine(_attackConfig.SwitchGizmosColor());
            }
            
            Machine.ChangeState(Player.IdleState);
        }

        public override void Tick()
        {
        }

        private IEnumerator AttackDelay()
        {
            _canAttack = false;
            yield return new WaitForSeconds(2f);
            _canAttack =  true;
        }
    }
}