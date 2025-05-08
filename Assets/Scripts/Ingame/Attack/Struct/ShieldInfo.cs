using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Attack.Struct
{
    public struct ShieldInfo
    {
        public IEntity Entity;
        public int Amount;
        public int Frame;

        public ShieldInfo(IEntity entity, int amount, int frame)
        {
            Entity = entity;
            Amount = amount;
            Frame = frame;
        }
    }
}