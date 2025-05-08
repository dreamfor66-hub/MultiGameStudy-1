using System;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff
{
    public static class BuffTriggerDispatcher
    {
        public static Action<HitTotalInfo> OnAttackHit;
        public static Action<IEntity, IEntity, BuffData> OnBuffApply;
        public static Action<IEntity, BuffInstance> OnBuffStackChanged;
        public static Action<IEntity, IEntity> OnDeath;
        public static Action<IEntity, IEntity> OnParryingHit;
        public static Action OnClearStage;

        public static void AttackHit(HitTotalInfo hitInfo) => OnAttackHit?.Invoke(hitInfo);
        public static void BuffApply(IEntity rootSource, IEntity target, BuffData buff) => OnBuffApply?.Invoke(rootSource, target, buff);
        public static void BuffStackChanged(IEntity owner, BuffInstance buff) => OnBuffStackChanged?.Invoke(owner, buff);
        public static void Death(IEntity attacker, IEntity victim) => OnDeath?.Invoke(attacker, victim);
        public static void ParryingHit(IEntity attacker, IEntity victim) => OnParryingHit?.Invoke(attacker, victim);
        public static void ClearStage() => OnClearStage?.Invoke();
    }
}