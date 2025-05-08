using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerDeath : BuffTriggerBase
    {
        public BuffTriggerDeath(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
        }

        protected override void OnStart()
        {
            BuffTriggerDispatcher.OnDeath += OnDeath;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnEnd()
        {
            BuffTriggerDispatcher.OnDeath -= OnDeath;
        }

        private void OnDeath(IEntity attacker, IEntity victim)
        {
            if (victim != Me)
                return;
            Invoke(Me);
        }
    }
}