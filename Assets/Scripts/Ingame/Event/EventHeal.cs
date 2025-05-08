using UnityEngine;

namespace Rogue.Ingame.Event
{
    public struct EventHeal : IEvent
    {
        public int Amount;
        public Vector3 Position;

        public EventHeal(int amount, Vector3 position)
        {
            Amount = amount;
            Position = position;
        }
    }
}