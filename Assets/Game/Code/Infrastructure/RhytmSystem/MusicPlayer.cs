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
        private float _timePositionMs;
        private float _trackLengthMs;

        protected override void Awake()
        {
            base.Awake();
            _trackLengthMs = music.clip.length * 1000f;
        }
        public float TrackLengthMs
        {
            get => _trackLengthMs;
            set => _trackLengthMs = value;
        }

        public void Play()
        {
            music.Play();
        }

        public void Pause()
        {
            music.Pause();
        }

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