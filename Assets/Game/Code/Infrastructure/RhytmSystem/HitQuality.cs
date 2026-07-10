using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.RhytmSystem
{
    public enum HitQuality
    {
        Miss,
        Bad,
        Good,
        Perfect
    }

    public static class HitQualityExtensions
    {
        private static readonly Dictionary<HitQuality, float> _multipliers = new()
        {
            { HitQuality.Miss, 0f},
            { HitQuality.Bad, 0.5f },
            { HitQuality.Good, 0.75f },
            { HitQuality.Perfect, 1f }
        };

        public static float GetMultiplier(this HitQuality quality)
        {
            return _multipliers.GetValueOrDefault(quality, 0f);
        }
    }
}