using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerHurt : BuffTriggerBase
    {
        public BuffTriggerHurt(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
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

        private void OnAttackHit(HitTotalInfo hit)
        {
            if (Me != hit.Main.Victim)
                return;
            if (hit.Detail.IsInstantDeath)
                return;
            if (hit.Damage.FinalDamage <= 0)
                return;
            //TODO: 기본 피해 != 0인데 다른 이유로(ex, 패링, 보호막) FinalDamage가 0이 된 케이스는 통과시키고 싶다

            var target = hit.Main.Attacker;
            Invoke(target);
        }
    }
}