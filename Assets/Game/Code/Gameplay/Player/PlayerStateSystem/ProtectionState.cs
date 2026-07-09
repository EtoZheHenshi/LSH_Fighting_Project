using System;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using Code.Infrastructure.RhytmSystem;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class ProtectionState : PlayerBaseState
    {
        private readonly Action _activeAction;
        
        protected override Action ActiveAction => _activeAction;
        
        public ProtectionState(PlayerController player, StateMachine machine, EventBusService eventBusService) 
            : base(player, machine, eventBusService)
        {
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
            Debug.Log("Protect");
            float protectTimeMs = Store.Instance.GetMusicPositionMs();
            float protectModifier = BeatTracker.Instance.CalculateHitMultiplier(protectTimeMs);
            
            if (protectModifier > 0) Debug.Log("Hit the beat!(Protect)");
            else
            {
                Debug.Log("Miss the beat!(Protect)");
            }
        }
        
        private void SwitchToAttack(SwitchPlayerRoles switchPlayerRole)
        {
            Machine.ChangeState(Player.AttackState);
        }
    }
}