using System;
using Code.Gameplay.Player.Attacks;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
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
            
            Player.PlayerIcons.SetRoleIcon(Color.red);
        }

        
        private void Attack()
        {
            _attackConfig.VisualizeAttack();

            // float accuracy = BeatTracker.Instance.CalculateHitAccuracy(attackTimeMs);
            // HitQuality attackQuality = BeatTracker.Instance.GetHitQuality(accuracy);
            // HitQuality protectQuality = Store.Instance.ProtectQuality;
            // float multiplier = attackQuality.GetMultiplier(protectQuality);
            //float accuracy = Store.Instance.ProtectAccuracy;
            // HitQuality attackQuality = Store.Instance.AttackQuality;
            // HitQuality protectQuality = Store.Instance.ProtectQuality;
            // float multiplier = Store.Instance.Multiplier;
            
            if (Store.Instance.AttackIsActive)
            {
                Debug.Log("Second attempt to attack per beat! Turning into protection!");
            }
            else
            {
                float attackTimeMs = Store.Instance.MusicPositionMs;

                Store.Instance.AttackTimeMs = attackTimeMs;

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
                BeatTracker.Instance.PlayerToHit = Player.Enemy;
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