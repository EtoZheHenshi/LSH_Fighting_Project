using System;
using Code.Gameplay.Player;
using Code.Gameplay.Player.Body;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay
{
    public class GameplayPoop
    {
        private readonly PlayerController _player1;
        private readonly PlayerController _player2;
        private readonly Transform _deadBodyRoot;
        private const float SwitchTime = 10f;
        private const float GhostTime = 5f;
        
        private EventBusService _eventBus;
        
        public GameplayPoop(PlayerController player1, PlayerController player2, Transform deadBodyRoot)
        {
            _player1 = player1;
            _player2 = player2;
            _deadBodyRoot = deadBodyRoot;
            _eventBus = EventBusService.Instance;
        }
        
        public async UniTask StartGameplayLoop()
        {
            _player1.StateMachine.ChangeState(_player1.GhostState);
            _player2.StateMachine.ChangeState(_player2.GhostState);
            
            await UniTask.Delay(TimeSpan.FromSeconds(GhostTime));
            
            FindDeadBody();
            
            _player1.StateMachine.ChangeState(_player1.AttackState);
            _player2.StateMachine.ChangeState(_player2.ProtectionState);
            
            TimerSwitch().Forget();
        }

        private async UniTask TimerSwitch()
        {
            while (true)
            {
                _eventBus.Publish(new SwitchPlayerRoles());
                await UniTask.Delay(TimeSpan.FromSeconds(SwitchTime));
            }
        }

        private void FindDeadBody()
        {
            DeadBody[] deadBodies = _deadBodyRoot.GetComponentsInChildren<DeadBody>();
            if (!_player1.HaveBody)
            {
                _player1.SetBody(deadBodies[0]);
            }

            if (!_player2.HaveBody)
            {
                _player2.SetBody(deadBodies[1]);
            }
        }
    }
}