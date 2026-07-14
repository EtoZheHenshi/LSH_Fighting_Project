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

            //Player.PlayerIcons.SetRoleIcon(Color.red);
        }

        private void Attack()
        {
            //_attackConfig.VisualizeAttack();
            Player.PlayerAnimator.SetTrigger("Attack");

            if (Store.Instance.AttackIsActive)
            {
                Player.FeedbackPopup.Play(HitQuality.Miss, Player.transform);
                BeatTracker.Instance.AttackQuality = HitQuality.Miss;
                BeatTracker.Instance.HitResult();
            }
            else
            {
                float attackTimeMs = Store.Instance.MusicPositionMs;

                HitQuality quality = BeatTracker.Instance.SetAttackQuality(attackTimeMs);
                
                Player.FeedbackPopup.Play(quality, Player.transform);

                Store.Instance.AttackIsActive = true;

                if (quality == HitQuality.Miss)
                {
                    BeatTracker.Instance.HitResult();
                }
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