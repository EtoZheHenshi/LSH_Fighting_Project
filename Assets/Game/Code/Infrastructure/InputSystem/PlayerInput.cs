using Code.Gameplay.Player;
using Code.InputSystem;
using UnityEngine.InputSystem;

namespace Code.Infrastructure.InputSystem
{
    public sealed class PlayerInput : IPlayerInput
    {
        private readonly InputActionMap _actions;
        
        public InputActionMap ActionMap => _actions;

        public float Move { get; set; }
        public bool Jump { get; set;}
        public bool Crouch { get; set;}
        public bool Attack { get; set;}

        public PlayerInput(GameInput.PlayerOneActions playerOneActions)
        {
            GameInput.PlayerOneActions actions = playerOneActions;
            _actions = actions;
            actions.Enable();
            
            actions.Move.performed += OnMove;
            actions.Move.canceled += OnMove;
            
            actions.Jump.started += OnJump;
            
            actions.Crouch.started += OnCrouch;
            actions.Crouch.canceled += OnCrouch;

            actions.HandAttack.started += OnHandAttack;
        }
        
        public PlayerInput(GameInput.PlayerTwoActions playerOneActions)
        {
            GameInput.PlayerTwoActions actions = playerOneActions;
            _actions = actions;
            actions.Enable();
            
            actions.Move.performed += OnMove;
            actions.Move.canceled += OnMove;
            
            actions.Jump.started += OnJump;
            
            actions.Crouch.started += OnCrouch;
            actions.Crouch.canceled += OnCrouch;

            actions.HandAttack.started += OnHandAttack;
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            Move = ctx.ReadValue<float>();
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            Jump = ctx.started;
        }

        private void OnCrouch(InputAction.CallbackContext ctx)
        {
            Crouch = ctx.started;
        }

        private void OnHandAttack(InputAction.CallbackContext ctx)
        {
            Attack = ctx.started;
        }
    }
}