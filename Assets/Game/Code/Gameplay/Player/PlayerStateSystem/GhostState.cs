using System;
using Code.Gameplay.Player.PlayerStateSystem.Base;
using Code.Infrastructure.EventBusSystem;
using UnityEngine;

namespace Code.Gameplay.Player.PlayerStateSystem
{
    public class GhostState : PlayerBaseState
    {
        private readonly Action _activeAction;
        private Collider2D[] _deadBodies;
        
        protected override Action ActiveAction => _activeAction;
        
        public GhostState(PlayerController player, StateMachine machine, EventBusService eventBusService) 
            : base(player, machine, eventBusService)
        {
            _activeAction = Possession;
            _deadBodies = new Collider2D[5];
        }

        public override void Enter()
        {
            base.Enter();
            
            Player.RemoveBody();
        }

        public override void Tick()
        {
            base.Tick();
            
            //_deadBodies = Physics2D.OverlapCircle(Player.transform.position, )
        }

        private void Possession()
        {
            Debug.Log("Possession");
            //логика завладевания телом
        }
    }
}