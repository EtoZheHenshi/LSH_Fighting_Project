namespace Code.Infrastructure.EventBusSystem.Events
{
    public class GameEndEvent : IEvent
    {
        public string WinPlayer;

        public GameEndEvent(string winPlayer)
        {
            WinPlayer = winPlayer;
        }
    }
}