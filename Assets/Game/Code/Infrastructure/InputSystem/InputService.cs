using Code.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Infrastructure.InputSystem
{
    public sealed class InputService : Singleton<InputService>
    {
        private GameInput _gameInput;
        
        public InputActionMap CurrentMap { get; private set; }
        public GameInput GameInput => _gameInput;

        protected override void Awake()
        {
            base.Awake();
            
            _gameInput = new GameInput();

            foreach (InputActionMap map in _gameInput.asset.actionMaps)
            {
                map.Disable();
            }
            
            DontDestroyOnLoad(gameObject);
        }

        public void SetMap(string mapName)
        {
            InputActionMap map = _gameInput.asset.FindActionMap(mapName);

            if (map == null)
            {
                Debug.LogWarning("Map not found: " + mapName);
                return;
            }
            
            CurrentMap?.Disable();
            CurrentMap = map;
            CurrentMap.Enable();
        }
    }
}