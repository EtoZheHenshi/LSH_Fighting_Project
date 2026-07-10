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
            _blockConfig.VisualizeAttack(Duration);

            float hitTimeMs = Store.Instance.GetMusicPositionMs();

            Store.Instance.ProtectTimeMs = hitTimeMs;

            float accuracy = BeatTracker.Instance.CalculateHitAccuracy(hitTimeMs);
            HitQuality quality = BeatTracker.Instance.HitQuality(accuracy);
            float multiplier = quality.GetMultiplier();


            if (accuracy < 0)
            {
                Debug.Log($"Miss the beat!(Protect)\naccuracy: {accuracy} | quality: {quality} | multiplier: {multiplier}");
                return;
            }
            
            ActivateBlock().Forget();
            Debug.Log($"Hit the beat!(Protect)\naccuracy: {accuracy} | quality: {quality} | multiplier: {multiplier}");
            
        }

        private async UniTask ActivateBlock()
        {
            _canBlock = true;
            await UniTask.Delay(TimeSpan.FromSeconds(Duration));
            _canBlock = false;
        }
        
        private void SwitchToAttack(SwitchPlayerRoles switchPlayerRole)
        {
            Machine.ChangeState(Player.AttackState);
        }
    }
}