using System;
using System.Threading;

namespace Rogue.Ingame.Goods
{
    public class MoneyModel
    {
        public int Amount { get; private set; }
        public Action OnChanged;

        public void Gain(int amount)
        {
            Amount += amount;
            OnChanged?.Invoke();
        }

        public void Use(int amount)
        {
            Amount -= amount;
            OnChanged?.Invoke();
        }

        public void Reset()
        {
            Amount = 0;
            OnChanged?.Invoke();
        }
    }
}