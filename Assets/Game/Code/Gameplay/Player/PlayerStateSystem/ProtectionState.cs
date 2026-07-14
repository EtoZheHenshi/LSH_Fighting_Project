using System;
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
        private const float Duration = 0.25f;
        private readonly Action _activeAction;
        private bool _canBlock;

        public bool CanBlock => _canBlock;

        protected override Action ActiveAction => _activeAction;

        public ProtectionState(PlayerController player, StateMachine machine,
            EventBusService eventBusService)
            : base(player, machine, eventBusService)
        {
            _activeAction = Protect;
        }

        public override void Enter()
        {
            base.Enter();

            //Player.PlayerIcons.SetRoleIcon(Color.blue);
            Player.ShieldSpriteRenderer.enabled = true;
        }

        public override void Exit()
        {
            base.Exit();
            
            Player.ShieldSpriteRenderer.enabled = false;
        }

        private void Protect()
        {
            if (!Store.Instance.ProtectIsActive)
            {
                ShieldBlink().Forget();
                
                Store.Instance.ProtectIsActive = true;

                float protectTimeMs = Store.Instance.MusicPositionMs;

                HitQuality quality = BeatTracker.Instance.SetProtectQuality(protectTimeMs);
                
                Player.FeedbackPopup.Play(quality, Player.transform);
            }
            else
            {
                Player.FeedbackPopup.Play(HitQuality.Miss, Player.transform);
            }
        }

        private async UniTask ShieldBlink()
        {
            Color color = Player.ShieldSpriteRenderer.color;
            color.a = 1f;
            Player.ShieldSpriteRenderer.color = color;

            await UniTask.Delay(200);
            
            color.a = 0.4f;
            Player.ShieldSpriteRenderer.color = color;
        }
    }
}