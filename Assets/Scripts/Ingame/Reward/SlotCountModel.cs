using System;

namespace Rogue.Ingame.Reward
{
    public class SlotCountModel
    {
        public int SlotCount { get; private set; } = 7;
        public Action OnChanged;

        public void AddSlot()
        {
            SlotCount++;
            OnChanged?.Invoke();
        }

        public void Reset()
        {
            SlotCount = 7;
        }
    }
}