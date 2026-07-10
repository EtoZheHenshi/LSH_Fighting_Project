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

        // логика передачи фазы атаки перехода в фазу защиты 
        private void TurnIntoProtectPhase()
        {
            
        }
        private void Attack()
        {
            _attackConfig.VisualizeAttack();

            float attackTimeMs = Store.Instance.MusicPositionMs;

            Store.Instance.AttackTimeMs = attackTimeMs;

            // float accuracy = BeatTracker.Instance.CalculateHitAccuracy(attackTimeMs);
            // HitQuality attackQuality = BeatTracker.Instance.GetHitQuality(accuracy);
            // HitQuality protectQuality = Store.Instance.ProtectQuality;
            // float multiplier = attackQuality.GetMultiplier(protectQuality);
            float accuracy = Store.Instance.ProtectAccuracy;
            HitQuality attackQuality = Store.Instance.AttackQuality;
            HitQuality protectQuality = Store.Instance.ProtectQuality;
            float multiplier = Store.Instance.Multiplier;

            if (accuracy < 0)
            {
                Debug.Log("Miss the beat!(Attack) " + multiplier);
                TurnIntoProtectPhase();
                GameplayPoop.Instance.SwitchPlayerRoles();
                return;
            }
            
            Debug.Log(
                $"Hit the beat!(Attack)\naccuracy: {accuracy} | quality: {quality} | multiplier: {multiplier}");
            
            if (Store.Instance.AttackIsActive)
            {
                Debug.Log("Second attempt to attack per beat! Turning into protection!");
                    
                TurnIntoProtectPhase();
            }
            else
            {
                Debug.Log("Hit the beat!(Attack)");
                // Debug.Log(
                // $"Hit the beat!(Attack)\naccuracy: {accuracy} | quality: {attackQuality} | multiplier: {multiplier}");
                Debug.Log($"ATTACK | AttakQuality: {attackQuality} | AttackMultiplier: {attackQuality.GetAttackMultiplier()}\n" +
                          $"ProtectQuality: {protectQuality} | ProtectMultiplier: {protectQuality.GetProtectMultiplier()} | " +
                          $"FinalMultiplier: {multiplier}");
                Store.Instance.AttackIsActive = true;
            }
            
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