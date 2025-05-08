using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveBuffAddTime : IBuffActive
    {
        private readonly BuffTag tag;
        private readonly int value;
        private readonly bool flag;

        public BuffActiveBuffAddTime(BuffActiveData data)
        {
            this.tag = data.Tag;
            this.value = data.Value;
            this.flag = data.IsDurationChanged;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            GameCommandBuffAddTime.Send(rootSource, target, new BuffIdentifer(tag, buff.Id), value, flag);
        }
    }
}