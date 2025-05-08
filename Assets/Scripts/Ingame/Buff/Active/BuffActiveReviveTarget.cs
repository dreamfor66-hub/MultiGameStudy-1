using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveReviveTargetHpPercent : IBuffActive
    {
        private readonly int percent;
        public BuffActiveReviveTargetHpPercent(BuffActiveData activeData)
        {
            percent = activeData.Value;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            GameCommandRevive.Send(target, percent);
        }
    }
}