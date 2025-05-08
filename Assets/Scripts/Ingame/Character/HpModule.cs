using System;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public struct HpInfo : IEquatable<HpInfo>
    {
        public int CurHp;
        public int MaxHp;
        public int Shield;

        public float Ratio => (float)CurHp / MaxHp;

        public bool Equals(HpInfo other)
        {
            return CurHp == other.CurHp && MaxHp == other.MaxHp && Shield == other.Shield;
        }

        public override bool Equals(object obj)
        {
            return obj is HpInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CurHp;
                hashCode = (hashCode * 397) ^ MaxHp;
                hashCode = (hashCode * 397) ^ Shield;
                return hashCode;
            }
        }
    }

    public class HpModule
    {
        public HpInfo HpInfo
        {
            get
            {
                if (syncedInfo.HasValue)
                    return syncedInfo.Value;
                return new HpInfo
                {
                    CurHp = curHp,
                    MaxHp = TotalMaxHp,
                    Shield = shield.TotalShield,
                };
            }
        }

        private readonly HpData hpData;
        private readonly BuffValues buffValues;

        private int TotalMaxHp => basicMaxHp + buffMaxHpValue + buffMaxHpPercent;
        private int basicMaxHp;
        private int curHp;

        private int buffMaxHpValue;
        private int buffMaxHpPercent;

        private HpInfo? syncedInfo;
        private readonly HpShield shield = new HpShield();

        public void Sync(HpInfo hpInfo)
        {
            syncedInfo = hpInfo;
        }

        public HpModule(HpData hpData, BuffValues buffValues)
        {
            this.hpData = hpData;
            this.buffValues = buffValues;
            basicMaxHp = hpData.BasicMaxHp;
            curHp = basicMaxHp;

            buffValues.Subscribe(BuffSimpleValueType.MaxHp, BuffMaxHpChanged, true);
            buffValues.Subscribe(BuffSimpleValueType.MaxHpPercent, BuffMaxHpPercentChanged, true);
        }

        public void SetPlayerCountHpBuff(float value)
        {
            this.basicMaxHp = (int)(hpData.BasicMaxHp * value);
            this.curHp = basicMaxHp;
        }

        public void UpdateFrame()
        {
            shield.UpdateFrame();
        }

        public void OnDestroy()
        {
            buffValues.UnSubscribe(BuffSimpleValueType.MaxHp, BuffMaxHpChanged);
            buffValues.UnSubscribe(BuffSimpleValueType.MaxHpPercent, BuffMaxHpPercentChanged);
        }

        /// <returns> killed  by this</returns>
        public bool InstantDeath()
        {
            if (curHp <= 0)
                return false;

            if (hpData.ImmuneInstantDeath)
                return false;

            curHp = 0;
            return true;
        }

        /// <returns> killed  by this</returns>
        public bool Attack(int damage)
        {
            if (curHp <= 0)
                return false;

            var damageAfterShield = shield.Hurt(damage);
            if (damageAfterShield >= curHp)
            {
                curHp = 0;
                return true;
            }
            else
            {
                curHp -= damageAfterShield;
                return false;
            }
        }

        /// <returns>실제 회복량</returns>
        public int Heal(int amount)
        {
            if (curHp <= 0)
                return 0;

            if (TotalMaxHp - curHp >= amount)
            {
                curHp += amount;
                return amount;
            }
            else
            {
                var realAmount = TotalMaxHp - curHp;
                curHp = TotalMaxHp;
                return realAmount;
            }
        }

        public void Shield(int amount, int frame)
        {
            shield.AddShield(amount, frame);
        }

        public void Revive(int percent)
        {
            curHp = Mathf.Clamp(TotalMaxHp * percent / 100, 1, TotalMaxHp);
        }

        private void BuffMaxHpChanged(int value)
        {
            var diff = value - buffMaxHpValue;

            if (curHp > 0 && diff > 0)
            {
                curHp += diff;
            }

            buffMaxHpValue = value;
        }

        private void BuffMaxHpPercentChanged(int value)
        {
            var calculated = basicMaxHp * value / 100;
            var diff = calculated - buffMaxHpPercent;
            if (curHp > 0 && diff > 0)
            {
                curHp += diff;
            }

            buffMaxHpPercent = calculated;
        }
    }
}