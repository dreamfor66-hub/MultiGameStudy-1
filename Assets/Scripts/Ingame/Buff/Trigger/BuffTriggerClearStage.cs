using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerClearStage : BuffTriggerBase
    {
        public BuffTriggerClearStage(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {

        }

        protected override void OnStart()
        {
            BuffTriggerDispatcher.OnClearStage += OnClear;

        }

        protected override void OnUpdate()
        {
        }

        protected override void OnEnd()
        {
            BuffTriggerDispatcher.OnClearStage -= OnClear;
        }

        private void OnClear()
        {
            Invoke(Me);
        }
    }
}