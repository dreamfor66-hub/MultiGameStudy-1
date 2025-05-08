using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveHeal : IBuffActive
    {
        private readonly int amount;

        public BuffActiveHeal(BuffActiveData data)
        {
            amount = data.Value;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            GameCommandHeal.Send(new HealInfo(target, amount));
        }
    }
}