using Rogue.Ingame.Character;
using UnityEngine;

namespace Rogue.Ingame.Event
{
    public struct EventAttackDo : IEvent
    {
        public CharacterBehaviour Character;
        public Transform Pivot;
        public Vector2 Range;
        public int Delay;

        public EventAttackDo(CharacterBehaviour character, Transform pivot, Vector2 range, int delay)
        {
            Character = character;
            Pivot = pivot;
            Range = range;
            Delay = delay;
        }
    }
}