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
        private float _beatCounter = 0;
        private float _nextBeatPosition;
        private float _lastBeatPosition;
        private float _lastHitPosition;
        private float _currentBeatPosition;

        // private int _activeBeat = -1;
        private readonly int _hitRadiusMs = 150;
        private float _activeBeatStartPosition;
        private float _activeBeatEndPosition;


        protected override void Awake()
        {
            base.Awake();
            _beatDurationMs = 60f / bpm * 1000;
            _nextBeatPosition = _beatDurationMs;
        }

        public float CalculateHitMultiplier(float timeMs)
        {
            float multiplier;
            print(timeMs + " " + _lastBeatPosition + " " + _nextBeatPosition);
            if (Mathf.Abs(timeMs - _lastBeatPosition) < _hitRadiusMs && _lastHitPosition != _lastBeatPosition)
            {
                multiplier = 1 - (Math.Abs(timeMs - _lastBeatPosition) / _hitRadiusMs);
                _lastHitPosition = _lastBeatPosition;
                return multiplier;
            }
            else if (Mathf.Abs(timeMs - _nextBeatPosition) < _hitRadiusMs && _lastHitPosition != _nextBeatPosition)
            {
                multiplier = 1 - (Math.Abs(timeMs - _nextBeatPosition) / _hitRadiusMs);
                _lastHitPosition = _nextBeatPosition;
                return multiplier;
            }

            return -1;
        }

        private void Update()
        {
            _currentBeatPosition = Store.Instance.GetMusicPositionMs();

            if (_currentBeatPosition >= _nextBeatPosition)
            {
                _beatCounter++;
                _lastBeatPosition = _nextBeatPosition;
                OnBeat?.Invoke();
                _nextBeatPosition += _beatDurationMs;
            }

            _activeBeatStartPosition = _nextBeatPosition - _hitRadiusMs;
            _activeBeatEndPosition = _nextBeatPosition + _hitRadiusMs;
        }
    }
}