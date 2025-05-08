using System;
using Rogue.Ingame.Buff.Active;
using Rogue.Ingame.Data.Buff;

namespace Rogue.Ingame.Buff.Factory
{
    public static class BuffActiveFactory
    {
        public static IBuffActive Create(BuffActiveData activeData)
        {
            switch (activeData.ActiveType)
            {
                case BuffActiveType.None:
                    return new BuffActiveNone();
                case BuffActiveType.BuffToTarget:
                    return new BuffActiveBuffToTarget(activeData);
                case BuffActiveType.SpawnBullet:
                    return new BuffActiveSpawnBullet(activeData);
                case BuffActiveType.InstantDeath:
                    return new BuffActiveInstantDeath();
                case BuffActiveType.JustDamage:
                    return new BuffActiveJustDamage(activeData);
                case BuffActiveType.Heal:
                    return new BuffActiveHeal(activeData);
                case BuffActiveType.ReviveRandomMemberHpPercent:
                    return new BuffActiveReviveRandomMemberHpPercent(activeData);
                case BuffActiveType.ForceAction:
                    return new BuffActiveForceAction(activeData);
                case BuffActiveType.AddStack:
                    return new BuffActiveAddStack(activeData);
                case BuffActiveType.AddShield:
                    return new BuffActiveAddShield(activeData);
                case BuffActiveType.ReviveTargetHpPercent:
                    return new BuffActiveReviveTargetHpPercent(activeData);
                case BuffActiveType.Stun:
                    return new BuffActiveStun(activeData);
                case BuffActiveType.GainStatus:
                    return new BuffActiveGainStatus(activeData);
                case BuffActiveType.BuffAddTime:
                    return new BuffActiveBuffAddTime(activeData);
                case BuffActiveType.BuffAddStack:
                    return new BuffActiveBuffAddStack(activeData);
                case BuffActiveType.BuffRelease:
                    return new BuffActiveReleaseBuff(activeData);
                case BuffActiveType.HealPercent:
                    return new BuffActiveHealPercent(activeData);
                case BuffActiveType.SpawnMonster:
                    return new BuffActiveSpawnMonster(activeData);
                case BuffActiveType.GainRootAggro:
                    return new BuffActiveGainRootAggro(activeData);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
