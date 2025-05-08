using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.EntityMessage
{
    public struct EntityMessageHit : IEntityMessage
    {
        public HitResultInfo HitResultInfo;

        public EntityMessageHit(HitResultInfo hitResultInfo)
        {
            HitResultInfo = hitResultInfo;
        }

    }
}