using System;
using UnityEngine;

namespace Code.Infrastructure.RhytmSystem
{
    /// <summary>
    /// Генератор ритмических событий на основе текущей позиции музыкального трека.
    /// Отслеживает биты (доли) и такты, вычисляет окна попадания для проверки тайминга игрока.
    /// Работает в связке с Store для получения актуальной позиции воспроизведения.
    /// В каждом кадре опрашивает Store.getMusicPosition() и, при достижении следующего бита,
    /// вызывает событие OnBeat. Окно попадания задаётся через _hitRadiusMs
    /// </summary>
    public sealed class BeatTracker : Singleton<BeatTracker>
    {
        [SerializeField] private int bpm = 120;
        public int BPM => bpm;
        public event Action OnBeat;

        // private float _beatDurationSec;
        private float _beatDurationMs;
        private float _nextBeatTime;
        private float _lastBeatCounter = 0;
        private float _nextBeatPosition;
        private float _currentBeatPosition;
        private int _activeBeat = -1;
        private int _hitRadiusMs = 100;
        private float _activeBeatStartPosition;
        private float _activeBeatEndPosition;


        protected override void Awake()
        {
            base.Awake();
            _beatDurationMs = 60f / bpm * 1000;
            _nextBeatPosition = _beatDurationMs;
        }

        [ContextMenu("HitMetronome")]
        private void HitUI()
        {
        }

        private void CheckTiming()
        {
        }
        // public bool IsPressInRhytm(out int quality){}
        private void Update()
        {
            _currentBeatPosition = Store.Instance.GetMusicPosition();
            if (_currentBeatPosition >= _nextBeatPosition)
            {
                _lastBeatCounter++;
                OnBeat?.Invoke();
                _nextBeatPosition += _beatDurationMs;
            }

            _activeBeatStartPosition = _nextBeatPosition - _hitRadiusMs;
            _activeBeatEndPosition = _nextBeatPosition + _hitRadiusMs;
        }
    }
}