using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff
{
    public interface IBuffActive
    {
        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff);
    }
}