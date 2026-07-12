using System;
using UnityEngine;

namespace Code.Infrastructure.InputSystem
{
    public interface IPlayerInput
    {
        public Vector2 Move { get; }
        public bool ActiveAction { get; }
        
        public Action InputActiveAction { get; set; }
        public void EnableInput();
        public void DisableInput();
    }
}