using System;
using Code.Gameplay.Player.Blocks;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class ProtectionState : PlayerBaseState
    {
        private readonly BlockConfig _blockConfig;
        private const float Duration = 0.5f;
        private readonly Action _activeAction;
        private bool _canBlock;
        private bool _delayActive;

        public bool CanBlock => _canBlock;

        protected override Action ActiveAction => _activeAction;
        
        public ProtectionState(PlayerController player, StateMachine machine,
            EventBusService eventBusService, BlockConfig blockConfig) 
            : base(player, machine, eventBusService)
        {
            _blockConfig = blockConfig;
            _activeAction = Protect;
        }
        
        public override void Enter()
        {
            base.Enter();
            
            EventBus.Subscribe<SwitchPlayerRoles>(SwitchToAttack);
        }

        public override void Exit()
        {
            base.Exit();
            
            EventBus.Unsubscribe<SwitchPlayerRoles>(SwitchToAttack);
        }

        private void Protect()
        {
            if (_delayActive) return;
            
            BlockDelay().Forget();
            
            _blockConfig.VisualizeAttack(Duration);
            
            if (true)//проверка на попадание в такт
            {
                ActivateBlock().Forget();
            }
        }

        private async UniTask ActivateBlock()
        {
            _canBlock = true;
            await UniTask.Delay(TimeSpan.FromSeconds(Duration));
            _canBlock = false;
        }
        
        private async UniTask BlockDelay()
        {
            _delayActive = true;
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            _delayActive = false;
        }
        
        private void SwitchToAttack(SwitchPlayerRoles switchPlayerRole)
        {
            Machine.ChangeState(Player.AttackState);
        }
    }
}