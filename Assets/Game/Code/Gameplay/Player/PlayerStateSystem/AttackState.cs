using System;
using Code.Gameplay.Player.Attacks;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using Code.Infrastructure.RhytmSystem;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class AttackState : PlayerBaseState
    {
        private readonly AttackConfig _attackConfig;
        private readonly LayerMask _enemyLayer;
        private readonly Action _activeAction;
        
        protected override Action ActiveAction => _activeAction;
        
        public AttackState(PlayerController player, StateMachine machine, EventBusService eventBusService,
            AttackConfig attackConfig, LayerMask enemyLayer) 
            : base(player, machine, eventBusService)
        {
            _attackConfig = attackConfig;
            _enemyLayer = enemyLayer;
            _activeAction = Attack;
        }

        public override void Enter()
        {
            base.Enter();
            
            EventBus.Subscribe<SwitchPlayerRoles>(SwitchToProtection);
        }

        public override void Exit()
        {
            base.Exit();
            
            EventBus.Unsubscribe<SwitchPlayerRoles>(SwitchToProtection);
        }

        private void Attack()
        {
            _attackConfig.VisualizeAttack();
            
            float attackTimeMs = Store.Instance.GetMusicPositionMs();
            float hitModifier = BeatTracker.Instance.CalculateHitMultiplier(attackTimeMs);

            if (hitModifier > 0)
            {
                Debug.Log("Hit the beat!(Attack) " + hitModifier);
                
                if (Physics2D.OverlapBox(
                        _attackConfig.transform.position,
                        _attackConfig.AttackSize,
                        0f,
                        _enemyLayer
                    )
                   )
                {
                    float damage = _attackConfig.Damage * hitModifier;
                    Player.Enemy.TakeDamage(damage);
                    Debug.Log($"Damage = {damage}");
                }
            }
            
            else
            {
                Debug.Log("Miss the beat!(Attack) " + hitModifier);
            }
            
        }

        private void SwitchToProtection(SwitchPlayerRoles switchPlayerRole)
        {
            Machine.ChangeState(Player.ProtectionState);
        }
    }
}