using System;
using UnityEngine;

namespace Code.Infrastructure.RhytmSystem
{
    /// <summary>
    /// Управляет воспроизведением музыкального трека (загрузка, запуск, пауза, остановка).
    /// Отвечает за синхронизацию текущего времени воспроизведения с Store.
    /// В будущем будет содержать ссылку на AudioSource и обновлять позицию в Store 
    /// через вызов Store.UpdateMusicPosition(float)
    /// </summary>
    public class MusicPlayer : Singleton<MusicPlayer>
    {
        public AudioSource music;
        public bool startPlaying;
        public BeatTracker beatTracker;
        private float _timePositionMs;

        private void Start()
        {
            Store.Instance.MusicPositionMs = music.time; 
        }

        private void Update()
        {
            Store.Instance.MusicPositionMs = music.time;
        }
    }
}