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

            float protectTimeMs = Store.Instance.MusicPositionMs;

            Store.Instance.ProtectTimeMs = protectTimeMs;

            // float accuracy = BeatTracker.Instance.CalculateHitAccuracy(protectTimeMs);
            // HitQuality quality = BeatTracker.Instance.GetHitQuality(accuracy);
            // Store.Instance.ProtectQuality = quality;
            // // float multiplier = quality.GetMultiplier();
            // HitQuality attackQuality = BeatTracker.Instance.GetHitQuality(accuracy);
            // HitQuality protectQuality = Store.Instance.ProtectQuality;
            // float multiplier = attackQuality.GetMultiplier(protectQuality);
            
            float accuracy = Store.Instance.ProtectAccuracy;
            HitQuality attackQuality = Store.Instance.AttackQuality;
            HitQuality protectQuality = Store.Instance.ProtectQuality;
            float multiplier = Store.Instance.Multiplier;

            if (accuracy < 0)
            {
                Debug.Log("Miss the beat!(Protect)");
                // Debug.Log($"Miss the beat!(Protect)\naccuracy: {accuracy} | quality: {quality}");
            }
            else
            {
                if (Store.Instance.ProtectIsActive)
                {
                    //
                }
                else
                {
                    Debug.Log("Hit the beat!(Protect)");
                    // Debug.Log($"Hit the beat!(Protect)\naccuracy: {accuracy} | quality: {quality}");

                    Debug.Log($"PROTECT | AttakQuality: {attackQuality} | AttackMultiplier: {attackQuality.GetAttackMultiplier()}\n" +
                              $"ProtectQuality: {protectQuality} | ProtectMultiplier: {protectQuality.GetProtectMultiplier()} | " +
                              $"FinalMultiplier: {multiplier}");
                    ActivateBlock().Forget();
                }
            }
            
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