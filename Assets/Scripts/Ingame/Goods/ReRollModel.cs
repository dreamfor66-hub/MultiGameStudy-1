using System;
using UnityEngine;

namespace Rogue.Ingame.Goods
{
    public class ReRollModel
    {
        private readonly MoneyModel moneyModel;
        public int Count { get; private set; }
        public int Cost => 0;
        //public int Cost => Mathf.RoundToInt(100 * Mathf.Pow(1.5f, Count));
        public bool CanReRoll => moneyModel.Amount >= Cost;
        public Action OnChanged;

        public ReRollModel(MoneyModel moneyModel)
        {
            this.moneyModel = moneyModel;
        }

        public void ReRoll()
        {
            moneyModel.Use(Cost);
            Count++;
            OnChanged?.Invoke();
        }

        public void Reset()
        {
            Count = 0;
            OnChanged?.Invoke();
        }
    }
}