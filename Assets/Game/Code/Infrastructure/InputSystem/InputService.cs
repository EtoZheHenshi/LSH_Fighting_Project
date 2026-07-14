using Code.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Infrastructure.InputSystem
{
    public sealed class InputService : Singleton<InputService>
    {
        private GameInput _gameInput;

        public PlayerInput Player1 { get; private set; }
        public PlayerInput Player2 { get; private set; }
        public GameInput GameInput => _gameInput;

        protected override void Awake()
        {
            base.Awake();
            
            _gameInput = new GameInput();

            // foreach (InputActionMap map in _gameInput.asset.actionMaps)
            // {
            //     map.Disable();
            // }
            
            DontDestroyOnLoad(gameObject);
            
            if (Gamepad.all.Count >= 1)
            {
                Player1 = new PlayerInput(_gameInput.GamepadOne);
            }
            else
            {
                Player1 = new PlayerInput(_gameInput.PlayerOne);
            }
            
            if (Gamepad.all.Count >= 2)
            {
                Player2 = new PlayerInput(_gameInput.GamepadTwo);
            }
            else
            {
                Player2 = new PlayerInput(_gameInput.PlayerTwo);
            }
        }
    }
}