using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.EntityMessage
{
    public struct EntityMessageHurt : IEntityMessage
    {
        public HitResultInfo HitResultInfo;

        public EntityMessageHurt(HitResultInfo hitResultInfo)
        {
            HitResultInfo = hitResultInfo;
        }
    }
}