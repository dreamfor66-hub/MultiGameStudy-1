using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Buff
{
    public enum HitSituation
    {
        Hit,
        Hurt,
    }

    public enum HitFilterType
    {
        TargetMoveSlow = 100,
        TargetIsBoss = 110,
        TargetAbnormalStatus = 120,
    }


    [System.Serializable]
    public struct HitFilterData
    {
        public HitFilterType Type;
    }


    public enum HitOverrideType
    {
        IncreaseDamagePercent = 0,
        IncreaseTrueDamageValuePerStack = 1,
        IncreaseDamagePercentByTargetHpRatioCurve = 2,
        CriticalChance = 10,
    }

    [System.Serializable]
    public struct HitOverrideData
    {
        public HitOverrideType Type;
        [ShowIf(nameof(Type), HitOverrideType.IncreaseDamagePercentByTargetHpRatioCurve)]
        public AnimationCurve Curve;

        [HideIf(nameof(Type), HitOverrideType.IncreaseDamagePercentByTargetHpRatioCurve)]
        public int Value;
    }


    [System.Serializable]
    public struct BuffEffectOverrideHitData
    {
        public HitSituation Situation;
        public List<HitFilterData> Filters;
        public HitOverrideData Override;

    }
}