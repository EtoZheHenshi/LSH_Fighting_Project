using System;
using Code.Gameplay.Player;
using Code.Infrastructure.EventBusSystem;
using Code.Infrastructure.EventBusSystem.Events;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay
{
    public class GameplayLoop
    {
        private readonly PlayerController _player1;
        private readonly PlayerController _player2;
        private const float SwitchTime = 10f;
        
        private EventBusService _eventBus;
        
        public GameplayLoop(PlayerController player1, PlayerController player2)
        {
            _player1 = player1;
            _player2 = player2;
            _eventBus = EventBusService.Instance;
        }
        
        public void StartGameplayLoop()
        {
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
    }
}