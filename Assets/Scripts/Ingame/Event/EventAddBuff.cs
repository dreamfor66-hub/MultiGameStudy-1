using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Event
{
    public struct EventChangeBuff : IEvent
    {
        public IEntity Entity;
        public BuffData Buff;
        public bool IsAdd;

        public EventChangeBuff(IEntity entity, BuffData buff, bool isAdd)
        {
            Entity = entity;
            Buff = buff;
            IsAdd = isAdd;
        }
    }
}