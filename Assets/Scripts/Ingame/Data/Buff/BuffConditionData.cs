using Rogue.Ingame.Entity;
using Sirenix.OdinInspector;
namespace Rogue.Ingame.Data.Buff
{
    public enum ComparisonOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public enum BuffConditionType
    {
        None,
        OwnerHpPercent,
        OwnerHasBuffTag,
        RootHasBuffTag,
        OwnerIsBoss,
        OwnerHasNotBuffTag,
    }

    [System.Serializable]
    public struct BuffConditionData
    {
        public BuffConditionType ConditionType;
        public bool ShowTag => ConditionType == BuffConditionType.OwnerHasBuffTag ||
                               ConditionType == BuffConditionType.RootHasBuffTag ||
                                ConditionType == BuffConditionType.OwnerHasNotBuffTag;
        public bool ShowOp => ConditionType == BuffConditionType.OwnerHpPercent ||
                               ConditionType == BuffConditionType.OwnerIsBoss;

        [ShowIf(nameof(ShowOp))]
        public ComparisonOperator Operator;

        [ShowIf(nameof(ConditionType), BuffConditionType.OwnerHpPercent)]
        public int Value;

        [ShowIf(nameof(ShowTag))]
        public BuffTag Tag;
    }
}