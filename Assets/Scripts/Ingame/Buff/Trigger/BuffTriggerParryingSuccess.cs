using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerParryingSuccess : BuffTriggerBase
    {
        public BuffTriggerParryingSuccess(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {

        }

        protected override void OnStart()
        {
            BuffTriggerDispatcher.OnParryingHit += OnParryingHit;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnEnd()
        {
            BuffTriggerDispatcher.OnParryingHit -= OnParryingHit;
        }

        private void OnParryingHit(IEntity attacker, IEntity victim)
        {
            if (victim != Me)
                return;
            Invoke(attacker);
        }
    }
}