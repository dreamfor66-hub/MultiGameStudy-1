using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveBuffAddStack : IBuffActive
    {
        private BuffTag tag;
        private int value;
        public BuffActiveBuffAddStack(BuffActiveData data)
        {
            this.tag = data.Tag;
            this.value = data.Value;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            GameCommandBuffAddStack.Send(me, target, new BuffIdentifer(tag, buff.Id), value);
        }
    }
}