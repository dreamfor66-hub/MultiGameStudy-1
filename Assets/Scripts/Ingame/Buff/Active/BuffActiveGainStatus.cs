using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveGainStatus : IBuffActive
    {
        private readonly CharacterStatusType type;
        private readonly int frame;
        public BuffActiveGainStatus(BuffActiveData data)
        {
            this.type = data.StatusType;
            this.frame = data.Frame;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            GameCommandGainStatus.Send(target, type, frame);
        }
    }
}