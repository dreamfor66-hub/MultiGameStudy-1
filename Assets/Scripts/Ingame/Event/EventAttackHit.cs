using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;

namespace Rogue.Ingame.Event
{
    public struct EventAttackHit : IEvent
    {
        public HitResultInfo HitInfo;
        public bool AttackerIsMe;
        public bool VictimIsMe;

        public EventAttackHit(HitResultInfo hitInfo, bool attackerIsMe, bool victimIsMe)
        {
            HitInfo = hitInfo;
            AttackerIsMe = attackerIsMe;
            VictimIsMe = victimIsMe;
        }
    }
}