namespace Code.Infrastructure.RhytmSystem
{
    public class InputCache : Singleton<InputCache>
    {
        private bool _isActive;
        
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
        
    }
}