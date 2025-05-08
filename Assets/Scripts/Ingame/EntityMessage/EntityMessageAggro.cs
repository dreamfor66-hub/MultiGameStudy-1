using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.EntityMessage
{
    public struct EntityMessageAggro : IEntityMessage
    {
        public IEntity Target;
        public float Value;

        public EntityMessageAggro(IEntity target, float value)
        {
            Target = target;
            Value = value;
        }

    }
}