namespace Code.Gameplay.Player.PlayerStateSystem.Base
{
    public interface IState
    {
        public void Enter();
        public void Tick();
        public void Exit();
    }
}