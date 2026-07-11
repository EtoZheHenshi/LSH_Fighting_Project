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
            
            if (!Store.Instance.ProtectIsActive)
            {
                Store.Instance.ProtectIsActive = true;

                float protectTimeMs = Store.Instance.MusicPositionMs;

                HitQuality quality = BeatTracker.Instance.SetProtectQuality(protectTimeMs);
                Debug.Log("ProtectQuality " + quality);
            }
        }
    }
}