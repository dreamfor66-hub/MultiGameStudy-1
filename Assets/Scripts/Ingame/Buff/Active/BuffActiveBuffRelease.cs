using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveReleaseBuff : IBuffActive
    {
        private BuffTag tag;
        public BuffActiveReleaseBuff(BuffActiveData data)
        {
            this.tag = data.Tag;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            GameCommandBuffRelease.Send(rootSource, target, new BuffIdentifer(tag, buff.Id));
        }
    }
}