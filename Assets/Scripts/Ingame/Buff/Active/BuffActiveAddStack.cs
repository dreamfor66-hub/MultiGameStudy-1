using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveAddStack : IBuffActive
    {
        private readonly int value;

        public BuffActiveAddStack(BuffActiveData data)
        {
            value = data.Value;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            GameCommandAddStack.Send(target, value);
        }
    }
}