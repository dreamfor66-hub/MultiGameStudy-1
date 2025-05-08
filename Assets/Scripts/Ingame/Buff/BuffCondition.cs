using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Buff
{
    public class BuffCondition
    {
        private readonly HpModule hp;
        private readonly bool isBoss;
        private IReadOnlyList<BuffInstance> buffs;
        public float HpRatio => hp?.HpInfo.Ratio ?? 1f;

        public BuffCondition(HpModule hpModule, bool isBoss)
        {
            hp = hpModule;
            this.isBoss = isBoss;
        }

        public bool IsConditionSatisfied(BuffConditionData conditionData, IEntity root)
        {
            switch (conditionData.ConditionType)
            {
                case BuffConditionType.None:
                    return true;
                case BuffConditionType.OwnerHpPercent:
                    {
                        return CheckFormula(conditionData, HpRatio * 100);
                    }
                case BuffConditionType.OwnerHasBuffTag:
                    return buffs.Any(x => x.Data.Tags.Any(t => t == conditionData.Tag));
                case BuffConditionType.RootHasBuffTag:
                    if (root is CharacterBehaviour character)
                        return character.BuffAccepter.GetBuffs().Any(x => x.Data.Tags.Any(t => t == conditionData.Tag));
                    throw new ArgumentException();
                case BuffConditionType.OwnerIsBoss:
                    return conditionData.Operator == ComparisonOperator.Equal ? isBoss : !isBoss;
                default:
                    throw new ArgumentException();
            }
        }

        private static bool CheckFormula(BuffConditionData condition, float realValue)
        {
            switch (condition.Operator)
            {
                case ComparisonOperator.Equal:
                    return condition.Value == Mathf.RoundToInt(realValue);
                case ComparisonOperator.NotEqual:
                    return condition.Value != Mathf.RoundToInt(realValue);
                case ComparisonOperator.GreaterThan:
                    return condition.Value < realValue;
                case ComparisonOperator.GreaterThanOrEqual:
                    return condition.Value <= realValue;
                case ComparisonOperator.LessThan:
                    return condition.Value > realValue;
                case ComparisonOperator.LessThanOrEqual:
                    return condition.Value >= realValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //Lazy init
        public void LinkBuffAccepter(IReadOnlyList<BuffInstance> buffList)
        {
            buffs = buffList;
        }
    }
}