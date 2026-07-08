using UnityEngine;

namespace Code.Infrastructure.InputSystem
{
    public interface IPlayerInput
    {
        public Vector2 Move { get; }
        public bool Attack { get; }
    }
}