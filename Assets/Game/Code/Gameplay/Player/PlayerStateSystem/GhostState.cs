using System;
using Code.Gameplay.Player.Body;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using Code.Infrastructure.RhytmSystem;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class GhostState : PlayerBaseState
    {
        private readonly LayerMask _deadBodyLayer;
        private readonly Action _activeAction;
        private Collider2D[] _deadBodies;
        private DeadBody _currentSelectedBody;
        
        protected override Action ActiveAction => _activeAction;
        
        public GhostState(PlayerController player, StateMachine machine,
            EventBusService eventBusService, LayerMask deadBodyLayer) 
            : base(player, machine, eventBusService)
        {
            _deadBodyLayer = deadBodyLayer;
            _activeAction = Possession;
            _deadBodies = new Collider2D[5];
        }

        public override void Enter()
        {
            base.Enter();

            Player.RemoveBody();
        }

        public override void Exit()
        {
            base.Exit();

            if (_currentSelectedBody != null)
            {
                _currentSelectedBody.SetOutline(false);
                _currentSelectedBody = null;
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (Player.HaveBody) return;

            if (_currentSelectedBody != null)
            {
                _currentSelectedBody.SetOutline(false);
            }
            
            _deadBodies = Physics2D.OverlapCircleAll(
                Player.transform.position,
                2f,
                _deadBodyLayer);
            
            if (_deadBodies.Length > 0)
            {
                _currentSelectedBody = GetNearestBody(_deadBodies).GetComponent<DeadBody>();
                _currentSelectedBody.SetOutline(true);
            }
            else
            {
                _currentSelectedBody = null;
            }
        }

        private void Possession()
        {
            float hitTimeMs = Store.Instance.GetMusicPositionMs();

            Store.Instance.AttackTimeMs = hitTimeMs;
            
            if (_currentSelectedBody != null && !Player.HaveBody)
            {
                Player.SetBody(_currentSelectedBody);
                GameplayPoop.Instance.RemoveDeadBodies(_currentSelectedBody);
                _currentSelectedBody.SetOutline(false);
                _currentSelectedBody = null;
            }
        }

        private Collider2D GetNearestBody(Collider2D[] colliders)
        {
            Collider2D nearest = null;
            float minDistance = float.MaxValue;

            foreach (var body in _deadBodies)
            {
                float distance = (body.transform.position - Player.transform.position).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = body;
                }
            }
            
            return nearest;
        }
    }
}