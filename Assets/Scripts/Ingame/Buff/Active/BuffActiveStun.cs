using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveStun : IBuffActive
    {
        private readonly int frame;
        public BuffActiveStun(BuffActiveData data)
        {
            this.frame = data.Frame;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            GameCommandStun.Send(target, frame);
        }
    }
}