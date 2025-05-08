using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveGainRootAggro : IBuffActive
    {
        private readonly float value;
        public BuffActiveGainRootAggro(BuffActiveData data)
        {
            this.value = data.Value;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            GameCommandGainRootAggro.Send(target, rootSource, value);
        }
    }
}