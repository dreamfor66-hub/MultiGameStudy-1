using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveAddShield : IBuffActive
    {
        private readonly int amount;
        private readonly int frame;
        public BuffActiveAddShield(BuffActiveData data)
        {
            amount = data.Value;
            frame = data.Frame;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            var shieldInfo = new ShieldInfo(target, amount, frame);
            GameCommandShield.Send(shieldInfo);
        }
    }
}