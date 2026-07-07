namespace Code.Infrastructure.InputSystem
{
    public interface IPlayerInput
    {
        public float Move { get; }
        public bool Jump { get; }
        public bool Crouch { get; }
        public bool Attack { get; }
    }
}