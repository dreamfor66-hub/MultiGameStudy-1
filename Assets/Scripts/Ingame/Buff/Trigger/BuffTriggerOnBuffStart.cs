using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerOnBuffStart : BuffTriggerBase
    {
        private bool isTriggered;
        public BuffTriggerOnBuffStart(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
            isTriggered = false;
        }

        protected override void OnStart()
        {
            if (isTriggered)
                return;
            Invoke(Me);
            isTriggered = true;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnEnd()
        {
        }
    }
}