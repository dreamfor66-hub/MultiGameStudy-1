using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Factory;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveInstantDeath : IBuffActive
    {
        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            var hitInfo = HitInfoFactory.CreateInstantDeath(rootSource, target);
            GameCommandHitAttack.Send(hitInfo);
        }
    }
}