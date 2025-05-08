using System.Linq;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerOnStackChange : BuffTriggerBase
    {
        private readonly BuffTag buffTag;
        private readonly int value;
        public BuffTriggerOnStackChange(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
            buffTag = triggerData.Tag;
            value = triggerData.Value;
        }

        protected override void OnStart()
        {
            BuffTriggerDispatcher.OnBuffStackChanged += OnStackChange;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnEnd()
        {
            BuffTriggerDispatcher.OnBuffStackChanged -= OnStackChange;
        }

        private void OnStackChange(IEntity owner, BuffInstance buff)
        {
            if (owner != Me)
                return;

            if (!buff.Data.Tags.Any(t => t == buffTag))
                return;

            if (buff.StackCount < value)
                return;

            Invoke(owner);
        }
    }
}