using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveBuffToTarget : IBuffActive
    {
        private readonly BuffData buffData;

        public BuffActiveBuffToTarget(BuffActiveData data)
        {
            this.buffData = data.Buff;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            GameCommandAddBuff.Send(rootSource, target, buffData);
        }
    }
}