using System;
using Code.Gameplay;
using Code.Gameplay.Player;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] private float perfectHitBound = 0.8f;
        [SerializeField] private float goodHitBound = 0.4f;
        [SerializeField] private int hitRadiusMs = 150;

        [SerializeField] private int missesCountToSwitchRole = 2;
        private int _missesCount;
        
        public Action OnBeat;

        public int BPM => bpm;
        public PlayerController PlayerToHit { get; set; }

        private HitQuality _attackQuality;
        private HitQuality _protectQuality;

        private float _beatDurationMs;
        private float _nextBeatTime;
        private float _nextBeatPosition;
        private float _lastBeatPosition;
        private float _lastAttackBeatPosition;
        private float _lastProtectBeatPosition;

        private float _currentBeatPosition;

        public int HitRadiusMs => hitRadiusMs;

        public HitQuality AttackQuality
        {
            get => _attackQuality;
            set => _attackQuality = value;
        }

        public float NextBeatPosition => _nextBeatPosition;

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

        public float CalculateHitAccuracy(float timeMs, bool isProtect)
        {
            float lastHitPosition;
            lastHitPosition = isProtect ? _lastProtectBeatPosition : _lastAttackBeatPosition;
            float multiplier;
            if (Mathf.Abs(timeMs - _lastBeatPosition) < hitRadiusMs &&
                lastHitPosition != _lastBeatPosition)
            {
                float mu = _lastBeatPosition;
                float sigma = hitRadiusMs / 2.5f;
                multiplier = NormalDistribution(timeMs, sigma, mu);
                if (isProtect)
                {
                    _lastProtectBeatPosition = _lastBeatPosition;
                }
                else
                {
                    _lastAttackBeatPosition = _lastBeatPosition;
                }
                return multiplier;
            }

            if (Mathf.Abs(timeMs - _nextBeatPosition) < hitRadiusMs &&
                lastHitPosition != _nextBeatPosition)
            {
                float mu = _nextBeatPosition;
                float sigma = hitRadiusMs / 2.5f;
                multiplier = NormalDistribution(timeMs, sigma, mu);
                if (isProtect)
                {
                    _lastProtectBeatPosition = _lastBeatPosition;
                }
                else
                {
                    _lastAttackBeatPosition = _lastBeatPosition;
                }
                return multiplier;
            }

            return -1;
        }

        public HitQuality GetHitQuality(float accuracy)
        {
            if (accuracy > perfectHitBound)
            {
                return HitQuality.Perfect;
            }
            else if (goodHitBound < accuracy && accuracy <= perfectHitBound)
            {
                return HitQuality.Good;
            }
            else if (0 < accuracy && accuracy <= goodHitBound)
            {
                return HitQuality.Bad;
            }

            return HitQuality.Miss;
        }

        private void Update()
        {
            _currentBeatPosition = Store.Instance.MusicPositionMs;
            if (_currentBeatPosition >= _nextBeatPosition)
            {
                HitResult();
            }
        }

        public void HitResult()
        {
            OnBeat?.Invoke();
            if (_attackQuality == HitQuality.Miss)
            {
                _missesCount++;
                if (_missesCount >= missesCountToSwitchRole)
                {
                    ResetData();
                    _missesCount = 0;
                    GameplayPoop.Instance.SwitchPlayerRoles();
                    return;   
                }
            }

            float multiplier = _attackQuality.GetMultiplier(_protectQuality);
            Store.Instance.Multiplier = multiplier;
            Debug.Log($"Multiplier: {multiplier}");


            if (PlayerToHit != null)
            {
                PlayerToHit.TakeDamage(PlayerToHit.Enemy.CurrentDamage * multiplier);
            }

            Debug.Log($"AttakQuality: {_attackQuality} | AttackMultiplier: {_attackQuality.GetAttackMultiplier()}\n" +
                      $"ProtectQuality: {_protectQuality} | ProtectMultiplier: {_protectQuality.GetProtectMultiplier()} | " +
                      $"FinalMultiplier: {multiplier}");
            // сбрасываем состояния 
            ResetData();
        }

        public HitQuality SetAttackQuality(float attackTimeMs)
        {
            float attackAccuracy = CalculateHitAccuracy(attackTimeMs, false);
            _attackQuality = GetHitQuality(attackAccuracy);
            return _attackQuality;
        }

        public HitQuality SetProtectQuality(float protectTimeMs)
        {
            float protectAccuracy = CalculateHitAccuracy(protectTimeMs, true);
            _protectQuality = GetHitQuality(protectAccuracy);
            return _protectQuality;
        }

        public void ResetData()
        {
            Store.Instance.AttackIsActive = false;
            Store.Instance.ProtectIsActive = false;
            PlayerToHit = null;

            _attackQuality = HitQuality.Null;
            _protectQuality = HitQuality.Null;
        
            _lastBeatPosition = _nextBeatPosition;
            _nextBeatPosition += _beatDurationMs;
        }
    }
}