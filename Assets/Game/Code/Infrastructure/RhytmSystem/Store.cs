using UnityEngine;

namespace Code.Infrastructure.RhytmSystem
{
    /// <summary>
    /// Глобальное хранилище данных, доступных другим системам.
    /// В текущей архитектуре предоставляет текущую позицию музыкального трека в миллисекундах.
    /// Служит посредником между MusicPlayer и Metronome.
    /// Метод getMusicPosition() должен возвращать актуальное время воспроизведения, 
    /// обновляемое из MusicPlayer .
    /// </summary>
    public class Store : Singleton<Store>
    {
        private float _musicPositionMs;

        public float GetMusicPositionMs()
        {
            return _musicPositionMs;
        }

        public void UpdateMusicPosition(float timeSec)
        {
            _musicPositionMs = timeSec * 1000f;
        }
    }
}