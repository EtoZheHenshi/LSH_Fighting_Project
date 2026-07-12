using System;
using Code.Infrastructure.Audio;
using UnityEngine;

namespace Code.Infrastructure.RhytmSystem
{
    /// <summary>
    /// Глобальное хранилище данных, доступных другим системам.
    /// В текущей архитектуре предоставляет текущую позицию музыкального трека в миллисекундах.
    /// Служит посредником между MusicPlayer и Metronome.
    /// Метод getMusicPosition() должен возвращать актуальное время воспроизведения, 
    /// обновляемое из MusicPlayer 
    /// </summary>
    public class Store : Singleton<Store>
    {
        private float _musicPositionMs;
        private int _loopCount;
        private float _lastAudioTime;

        private bool _attackIsActive = false;
        private bool _protectIsActive = false;
        private float _lastBeatPositionMs;
        private HitQuality _attackQuality = HitQuality.Miss;
        private HitQuality _protectQuality = HitQuality.Miss;
        private float _multiplier;

        public float Multiplier
        {
            get => _multiplier;
            set => _multiplier = value;
        }

        public HitQuality AttackQuality
        {
            get => _attackQuality;
            set => _attackQuality = value;
        }

        public HitQuality ProtectQuality
        {
            get => _protectQuality;
            set => _protectQuality = value;
        }

        public bool AttackIsActive
        {
            get => _attackIsActive;
            set
            {
                // AttackQuality = HitQuality.Miss;
                _attackIsActive = value;
            }
        }

        public bool ProtectIsActive
        {
            get => _protectIsActive;

            set
            {
                // ProtectQuality = HitQuality.Miss;
                _protectIsActive = value;
            }
        }

        public float MusicPositionMs
        {
            get => _musicPositionMs;
            set => _musicPositionMs = value * 1000f;
        }

        private void Update()
        {
            if (AudioManager.Instance.PlayerMusicSet)
            {
                MusicPositionMs = AudioManager.Instance.PlayerMusicSource.time;

                float current = AudioManager.Instance.PlayerMusicSource.time * 1000f;

                if (current < _lastAudioTime)
                {
                    _loopCount++;
                }

                _lastAudioTime = current;

                _musicPositionMs =
                    current +
                    _loopCount * AudioManager.Instance.PlayerTrackLengthMs;
            }
        }
    }
}