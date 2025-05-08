using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Factory;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveJustDamage : IBuffActive
    {
        private readonly HitDamageType type;
        private readonly int damage;
        private readonly bool isStacked;

        public BuffActiveJustDamage(BuffActiveData data)
        {
            type = data.DamageType;
            damage = data.Value;
            isStacked = data.IsStacked;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            var hitInfo = HitInfoFactory.CreateJustDamage(type, damage * (isStacked ? buff.StackCount : 1), target, rootSource);
            GameCommandHitAttack.Send(hitInfo);
        }
    }
}