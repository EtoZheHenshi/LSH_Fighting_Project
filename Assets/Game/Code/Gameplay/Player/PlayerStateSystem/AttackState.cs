using System;
using Code.Gameplay.Player.Attacks;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class AttackState : PlayerBaseState
    {
        private readonly AttackConfig _attackConfig;
        private readonly LayerMask _enemyLayer;
        private readonly Action _activeAction;
        private bool _delayActive;

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
            if (_delayActive) return;
            
            AttackDelay().Forget();
            
            _attackConfig.VisualizeAttack();
            
            if (false) //Проверка на попадание в такт.
            {
                EventBus.Publish(new SwitchPlayerRoles());
                return;
            }

            if (Physics2D.OverlapBox(
                    _attackConfig.transform.position,
                    _attackConfig.AttackSize,
                    0f,
                    _enemyLayer
                )
               )
            {
                Player.Enemy.TakeDamage(_attackConfig.Damage);
            }
        }
        
        private async UniTask AttackDelay()
        {
            _delayActive = true;
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            _delayActive = false;
        }

        private void SwitchToProtection(SwitchPlayerRoles switchPlayerRole)
        {
            Machine.ChangeState(Player.ProtectionState);
        }
    }
}