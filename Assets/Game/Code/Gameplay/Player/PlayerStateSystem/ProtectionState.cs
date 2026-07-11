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
            
            Player.PlayerIcons.SetRoleIcon(Color.blue);
        }

        private void Protect()
        {
            _blockConfig.VisualizeAttack(Duration);
            
            // float accuracy = BeatTracker.Instance.CalculateHitAccuracy(protectTimeMs);
            // HitQuality quality = BeatTracker.Instance.GetHitQuality(accuracy);
            // Store.Instance.ProtectQuality = quality;
            // // float multiplier = quality.GetMultiplier();
            // HitQuality attackQuality = BeatTracker.Instance.GetHitQuality(accuracy);
            // HitQuality protectQuality = Store.Instance.ProtectQuality;
            // float multiplier = attackQuality.GetMultiplier(protectQuality);
            //float accuracy = Store.Instance.ProtectAccuracy;
            
            HitQuality attackQuality = Store.Instance.AttackQuality;
            HitQuality protectQuality = Store.Instance.ProtectQuality;
            float multiplier = Store.Instance.Multiplier;

            if (!Store.Instance.ProtectIsActive)
            {
                Store.Instance.ProtectIsActive = true;
                
                float protectTimeMs = Store.Instance.MusicPositionMs;

                Store.Instance.ProtectTimeMs = protectTimeMs;
            }
            
        }
    }
}