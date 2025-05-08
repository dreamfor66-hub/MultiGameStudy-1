using System;
using Rogue.Ingame.Buff.Trigger;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Factory
{
    public static class BuffTriggerFactory
    {
        public static BuffTriggerBase Create(IEntity me, BuffTriggerData triggerData)
        {
            switch (triggerData.TriggerType)
            {
                case BuffTriggerType.EveryValueFrame:
                    return new BuffTriggerEveryValueFrame(me, triggerData);
                case BuffTriggerType.OnBuffStart:
                    return new BuffTriggerOnBuffStart(me, triggerData);
                case BuffTriggerType.AttackDo:
                    return new BuffTriggerAttackDo(me, triggerData);
                case BuffTriggerType.AttackHitTarget:
                    return new BuffTriggerAttackHit(me, triggerData);
                case BuffTriggerType.AttackHitTargetCritical:
                    return new BuffTriggerAttackHitCritical(me, triggerData);
                case BuffTriggerType.ApplySpecificBuff:
                    return new BuffTriggerApplySpecificBuff(me, triggerData);
                case BuffTriggerType.BuffStackGreaterOrEqualThanValue:
                    return new BuffTriggerOnStackChange(me, triggerData);
                case BuffTriggerType.Hurt:
                    return new BuffTriggerHurt(me, triggerData);
                case BuffTriggerType.SingleHurtMoreThanHpPercent:
                    return new BuffTriggerSingleHurtMoreThanHpPercent(me, triggerData);
                case BuffTriggerType.CumulativeDamageMoreThanHpPercent:
                    return new BuffTriggerCumulativeDamageMoreThanHpPercent(me, triggerData);
                case BuffTriggerType.Death:
                    return new BuffTriggerDeath(me, triggerData);
                case BuffTriggerType.ParryingSuccess:
                    return new BuffTriggerParryingSuccess(me, triggerData);
                case BuffTriggerType.KillTarget:
                    return new BuffTriggerKillTarget(me, triggerData);
                case BuffTriggerType.ClearStage:
                    return new BuffTriggerClearStage(me, triggerData);
                case BuffTriggerType.ActionMoveMilliMeter:
                    return new BuffTriggerActionMoveMilliMeter(me, triggerData);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}