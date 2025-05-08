using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Event
{
    public struct EventCoinGet : IEvent
    {
        public int Amount;
        public IEntity Entity;

        public EventCoinGet(int amount, IEntity entity)
        {
            Amount = amount;
            Entity = entity;
        }
    }
}