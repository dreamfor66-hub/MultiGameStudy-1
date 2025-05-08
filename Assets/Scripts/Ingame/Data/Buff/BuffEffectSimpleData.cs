using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data.Buff
{
    public enum BuffSimpleValueType
    {
        MoveSpeedPercent = 0,
        Freeze = 10,
        Hide = 11,

        MaxHp = 20,
        MaxHpPercent = 21,

        AttackDamage = 40,

        AttackDamagePercent = 41,

        AttackSpeedPercent = 51,

        HealAmountPercent = 61,

        HurtDamagePercent = 71,
    }

    public enum BuffSimpleStackType
    {
        None,
        Multiply,
    }

    public enum BuffSimpleCurveType
    {
        None,
        HpRatio,
    }

    [System.Serializable]
    public struct BuffEffectSimpleData
    {
        public BuffSimpleValueType ValueType;
        public int Value;
        public BuffSimpleStackType StackType;

        public BuffSimpleCurveType CurveType;
        [HideIf(nameof(CurveType), BuffSimpleCurveType.None)]
        public AnimationCurve Curve;
    }
}