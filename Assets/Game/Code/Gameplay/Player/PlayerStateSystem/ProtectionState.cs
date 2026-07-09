using System;
using Code.Gameplay.Player.Blocks;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using Code.Infrastructure.RhytmSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class ProtectionState : PlayerBaseState
    {
        private readonly BlockConfig _blockConfig;
        private const float Duration = 0.25f;
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
            // if (_delayActive) return;
            //
            // BlockDelay().Forget();
            
            _blockConfig.VisualizeAttack(Duration);
            
            float protectTimeMs = Store.Instance.GetMusicPositionMs();
            float protectModifier = BeatTracker.Instance.CalculateHitMultiplier(protectTimeMs);

            if (protectModifier > 0)
            {
                ActivateBlock().Forget();
            }
            else
            {
                Debug.Log("Miss the beat!(Protect)");
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