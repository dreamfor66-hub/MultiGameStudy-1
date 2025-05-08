using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class ShieldInfo
    {
        public int RemainAmount;
        public int RemainFrame;

        public ShieldInfo(int amount, int frame)
        {
            RemainAmount = amount;
            RemainFrame = frame;
        }
    }

    public class HpShield
    {
        public int TotalShield => shields.Sum(x => x.RemainAmount);

        private readonly List<ShieldInfo> shields = new List<ShieldInfo>();

        public void UpdateFrame()
        {
            foreach (var info in shields)
                info.RemainFrame--;
            shields.RemoveAll(x => x.RemainFrame <= 0);
        }

        public void AddShield(int amount, int frame)
        {
            shields.Add(new ShieldInfo(amount, frame));
            shields.Sort(Comparison);
        }

        public int Comparison(ShieldInfo left, ShieldInfo right)
        {
            if (left.RemainFrame < right.RemainFrame)
                return -1;
            else if (left.RemainFrame > right.RemainFrame)
                return 1;
            else
                return 0;
        }

        public int Hurt(int damage)
        {
            var remainDamage = damage;
            foreach (var shield in shields)
            {
                var min = Mathf.Min(shield.RemainAmount, remainDamage);
                shield.RemainAmount -= min;
                remainDamage -= min;
                if (remainDamage <= 0)
                    break;
            }

            shields.RemoveAll(x => x.RemainAmount <= 0);
            return remainDamage;
        }
    }
}