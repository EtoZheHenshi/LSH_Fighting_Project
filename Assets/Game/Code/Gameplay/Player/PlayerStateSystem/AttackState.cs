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
                GameplayPoop.Instance.SwitchPlayerRoles();
                return;
            }
            
            Debug.Log(
                $"Hit the beat!(Attack)\naccuracy: {accuracy} | quality: {quality} | multiplier: {multiplier}");
            
            if (Physics2D.OverlapBox(
                    _attackConfig.AttackPosition.position,
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
        
        public void DrawGizmos()
        {
            if (_attackConfig == null)
                return;

            Gizmos.color = Color.red;

            Matrix4x4 oldMatrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(
                _attackConfig.AttackPosition.position,
                _attackConfig.transform.rotation,
                Vector3.one);

            Gizmos.DrawWireCube(Vector3.zero, _attackConfig.AttackSize);

            Gizmos.matrix = oldMatrix;
        }
    }
}