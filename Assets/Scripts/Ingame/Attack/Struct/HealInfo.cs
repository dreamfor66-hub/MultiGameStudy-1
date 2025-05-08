using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Attack.Struct
{
    public struct HealResultInfo
    {
        public IEntity Target;
        public int RealAmount;

        public HealResultInfo(IEntity target, int realAmount)
        {
            Target = target;
            RealAmount = realAmount;
        }
    }

    public struct HealInfo
    {
        public IEntity Target;
        public int Amount;

        public HealInfo(IEntity target, int amount)
        {
            Target = target;
            Amount = amount;
        }
    }
}