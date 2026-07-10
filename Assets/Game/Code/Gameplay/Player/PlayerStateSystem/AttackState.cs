using System;
using Code.Gameplay.Player.Attacks;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using Code.Infrastructure.RhytmSystem;
using Cysharp.Threading.Tasks;
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

            float hitTimeMs = Store.Instance.GetMusicPositionMs();

            Store.Instance.AttackTimeMs = hitTimeMs;
            
            float accuracy = BeatTracker.Instance.CalculateHitAccuracy(hitTimeMs);
            HitQuality quality = BeatTracker.Instance.HitQuality(accuracy);
            float multiplier = quality.GetMultiplier();


            if (accuracy < 0)
            {
                Debug.Log("Miss the beat!(Attack) " + multiplier);
                EventBus.Publish(new SwitchPlayerRoles());
                return;
            }
            else
            {
                Debug.Log(
                    $"Hit the beat!(Attack)\naccuracy: {accuracy} | quality: {quality} | multiplier: {multiplier}");
                if (Physics2D.OverlapBox(
                        _attackConfig.transform.position,
                        _attackConfig.AttackSize,
                        0f,
                        _enemyLayer
                    )
                   )
                {
                    float damage = _attackConfig.Damage * multiplier;

                    Player.Enemy.TakeDamage(damage);
                    Debug.Log($"Damage = {damage}");
                }
            }
        }

        // private async UniTask AttackDelay()
        // {
        //     _delayActive = true;
        //     await UniTask.Delay(TimeSpan.FromSeconds(2));
        //     _delayActive = false;
        // }

        private void SwitchToProtection(SwitchPlayerRoles switchPlayerRole)
        {
            Machine.ChangeState(Player.ProtectionState);
        }
    }
}