using UnityEngine;

namespace Rogue.Ingame.Event
{
    public struct EventDamage : IEvent
    {
        public int Damage;
        public bool Critical;
        public Vector3 Position;

        public EventDamage(int damage, bool critical, Vector3 position)
        {
            Damage = damage;
            Critical = critical;
            Position = position;
        }
    }
}