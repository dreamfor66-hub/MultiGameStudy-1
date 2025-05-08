using System.Linq;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerApplySpecificBuff : BuffTriggerBase
    {
        private readonly BuffTag buffTag;
        public BuffTriggerApplySpecificBuff(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
            buffTag = triggerData.Tag;
        }

        protected override void OnStart()
        {
            BuffTriggerDispatcher.OnBuffApply += OnBuffApply;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnEnd()
        {
            BuffTriggerDispatcher.OnBuffApply -= OnBuffApply;
        }

        private void OnBuffApply(IEntity rootSource, IEntity target, BuffData buff)
        {
            if (rootSource != Me)
                return;

            if (!buff.Tags.Any(t => t == buffTag))
                return;

            Invoke(target);
        }
    }
}