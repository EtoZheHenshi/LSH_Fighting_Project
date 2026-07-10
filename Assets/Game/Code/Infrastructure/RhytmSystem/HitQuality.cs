using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.RhytmSystem
{
    public enum HitQuality
    {
        Null,
        Miss,
        Bad,
        Good,
        Perfect
    }

    public static class HitQualityExtensions
    {
        private static readonly Dictionary<HitQuality, float> _attackMultipliers = new()
        {
            { HitQuality.Bad, 0.4f },
            { HitQuality.Good, 0.8f },
            { HitQuality.Perfect, 1.2f }
        };
        private static readonly Dictionary<HitQuality, float> _protectMultipliers = new()
        {
            { HitQuality.Miss, 1f},
            { HitQuality.Bad, 0.75f },
            { HitQuality.Good, 0.5f },
            { HitQuality.Perfect, 0f }
        };

        public static float GetMultiplier(this HitQuality attackQuality, HitQuality protectQuality)
        {
            float attackMul = _attackMultipliers.GetValueOrDefault(attackQuality, 0f);
            float protectMul = _protectMultipliers.GetValueOrDefault(protectQuality, 1f);
            return attackMul *  protectMul;
        }
        
        public static float GetAttackMultiplier(this HitQuality attackQuality)
        {
            float attackMul = _attackMultipliers.GetValueOrDefault(attackQuality, 0f);
            return attackMul;
        }
        
        public static float GetProtectMultiplier(this HitQuality protectQuality)
        {
            float protectMul = _protectMultipliers.GetValueOrDefault(protectQuality, 0f);
            return protectMul;
        }
    }
}