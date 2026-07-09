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

        public Action AttackAction { get; set; }

        public PlayerInput(GameInput.PlayerOneActions playerOneActions)
        {
            GameInput.PlayerOneActions actions = playerOneActions;
            _actions = actions;
            actions.Enable();
            
            actions.Move.performed += OnMove;
            actions.Move.canceled += OnMove;
            
            // actions.ActiveAction.started += OnHandAttack;
            // actions.ActiveAction.canceled += OnHandAttack;
            
            actions.ActiveAction.performed += _ => AttackAction?.Invoke();
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
            
            actions.ActiveAction.performed += _ => AttackAction?.Invoke();
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