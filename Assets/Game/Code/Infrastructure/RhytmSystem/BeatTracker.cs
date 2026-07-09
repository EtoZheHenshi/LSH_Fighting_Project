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

        [SerializeField] private float perfectHitBound = 0.8f;
        [SerializeField] private float goodHitBound = 0.6f;
        [SerializeField] private float badHitBound = 0.2f;
        private readonly int _hitRadiusMs = 150;

        private float _beatDurationMs;
        private float _nextBeatTime;
        private float _nextBeatPosition;
        private float _lastBeatPosition;
        private float _lastHitPosition;

        private float _currentBeatPosition;

        protected override void Awake()
        {
            base.Awake();
            _beatDurationMs = 60f / bpm * 1000;
            _nextBeatPosition = _beatDurationMs;
        }

        private float NormalDistribution(float x, float sigma, float mu)
        {
            return Mathf.Exp(-((x - mu) * (x - mu) / (2 * sigma * sigma)));
        }

        public float CalculateHitAccuracy(float timeMs)
        {
            float multiplier;
            print($"timeMs: {timeMs}, _lastBeatPosition: {_lastBeatPosition}, _nextBeatPosition: {_nextBeatPosition}");
            if (Mathf.Abs(timeMs - _lastBeatPosition) < _hitRadiusMs &&
                _lastHitPosition != _lastBeatPosition)
            {
                float mu = _lastBeatPosition;
                float sigma = _hitRadiusMs / 2.5f;
                multiplier = NormalDistribution(timeMs, sigma, mu);
                _lastHitPosition = _lastBeatPosition;
                return multiplier;
            }
            else if (Mathf.Abs(timeMs - _nextBeatPosition) < _hitRadiusMs &&
                     _lastHitPosition != _nextBeatPosition)
            {
                float mu = _nextBeatPosition;
                float sigma = _hitRadiusMs / 2.5f;
                multiplier = NormalDistribution(timeMs, sigma, mu);
                _lastHitPosition = _nextBeatPosition;
                return multiplier;
            }

            return -1;
        }

        public HitQuality HitQuality(float multiplier)
        {
            if (multiplier > perfectHitBound)
            {
                return RhytmSystem.HitQuality.Perfect;
            }
            else if (badHitBound < multiplier && multiplier <= goodHitBound)
            {
                return RhytmSystem.HitQuality.Good;
            }
            else if (0 < multiplier && multiplier <= badHitBound)
            {
                return RhytmSystem.HitQuality.Bad;
            }

            return RhytmSystem.HitQuality.Miss;
        }

        private void Update()
        {
            _currentBeatPosition = Store.Instance.GetMusicPositionMs();

            if (_currentBeatPosition >= _nextBeatPosition)
            {
                _lastBeatPosition = _nextBeatPosition;
                OnBeat?.Invoke();
                _nextBeatPosition += _beatDurationMs;
            }
        }
    }
}