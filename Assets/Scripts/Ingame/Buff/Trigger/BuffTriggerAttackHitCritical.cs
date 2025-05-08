using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerAttackHitCritical : BuffTriggerBase
    {
        private readonly ActionTypeMask typeMask;

        public BuffTriggerAttackHitCritical(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
            typeMask = triggerData.AttackType;
        }

        protected override void OnStart()
        {
            BuffTriggerDispatcher.OnAttackHit += OnAttackHit;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnEnd()
        {
            BuffTriggerDispatcher.OnAttackHit -= OnAttackHit;
        }

        private void OnAttackHit(HitTotalInfo hitInfo)
        {
            if (hitInfo.Main.Attacker != Me && hitInfo.Main.AttackerRoot != Me)
                return;

            if (!ActionTypeHelper.CheckType(typeMask, hitInfo.Detail.ActionType))
                return;

            if (!hitInfo.Detail.IsDirect && !TriggerData.TriggerOnIndirectHit)
                return;

            if (hitInfo.Damage.IsCritical)
                Invoke(hitInfo.Main.Victim);
        }
    }
}