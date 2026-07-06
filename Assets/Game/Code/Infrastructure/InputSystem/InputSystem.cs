using Code.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Infrastructure.InputSystem
{
    public sealed class InputSystem
    {
        private readonly GameInput _gameInput;
        
        public InputActionMap CurrentMap { get; private set; }
        public GameInput GameInput => _gameInput;

        public InputSystem()
        {
            _gameInput = new GameInput();

            foreach (InputActionMap map in _gameInput.asset.actionMaps)
            {
                map.Disable();
            }
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