using System;
using Code.Gameplay.Player;
using Code.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Infrastructure.InputSystem
{
    public sealed class PlayerInput : IPlayerInput
    {
        private readonly InputActionMap _actions;
        
        public InputActionMap ActionMap => _actions;

        public Vector2 Move { get; set; }
        public bool ActiveAction { get; set;}

        public Action InputActiveAction { get; set; }
        public void EnableInput()
        {
            _actions.Enable();
        }

        public void DisableInput()
        {
            _actions.Disable();
        }

        public PlayerInput(GameInput.PlayerOneActions playerOneActions)
        {
            GameInput.PlayerOneActions actions = playerOneActions;
            _actions = actions;
            actions.Enable();
            
            actions.Move.performed += OnMove;
            actions.Move.canceled += OnMove;
            
            // actions.ActiveAction.started += OnHandAttack;
            // actions.ActiveAction.canceled += OnHandAttack;
            
            actions.ActiveAction.performed += _ => InputActiveAction?.Invoke();
        }
        
        public PlayerInput(GameInput.PlayerTwoActions playerOneActions)
        {
            GameInput.PlayerTwoActions actions = playerOneActions;
            _actions = actions;
            actions.Enable();
            
            actions.Move.performed += OnMove;
            actions.Move.canceled += OnMove;

            // actions.ActiveAction.started += OnHandAttack;
            // actions.ActiveAction.canceled += OnHandAttack;
            
            actions.ActiveAction.performed += _ => InputActiveAction?.Invoke();
        }
        
        public PlayerInput(GameInput.GamepadOneActions playerOneActions)
        {
            GameInput.GamepadOneActions actions = playerOneActions;
            actions.Get().devices = new InputDevice[]
            {
                Gamepad.all[0]
            };
            _actions = actions;
            actions.Enable();
            
            actions.Move.performed += OnMove;
            actions.Move.canceled += OnMove;

            // actions.ActiveAction.started += OnHandAttack;
            // actions.ActiveAction.canceled += OnHandAttack;
            
            actions.ActiveAction.performed += _ => InputActiveAction?.Invoke();
        }
        
        public PlayerInput(GameInput.GamepadTwoActions playerOneActions)
        {
            GameInput.GamepadTwoActions actions = playerOneActions;
            actions.Get().devices = new InputDevice[]
            {
                Gamepad.all[1]
            };
            _actions = actions;
            actions.Enable();
            
            actions.Move.performed += OnMove;
            actions.Move.canceled += OnMove;

            // actions.ActiveAction.started += OnHandAttack;
            // actions.ActiveAction.canceled += OnHandAttack;
            
            actions.ActiveAction.performed += _ => InputActiveAction?.Invoke();
        }



        private void OnMove(InputAction.CallbackContext ctx)
        {
            Move = ctx.ReadValue<Vector2>();
        }

        private void OnHandAttack(InputAction.CallbackContext ctx)
        {
            ActiveAction = ctx.started;
        }
    }
}