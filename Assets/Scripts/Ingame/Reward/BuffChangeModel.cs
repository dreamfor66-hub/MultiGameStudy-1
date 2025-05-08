using System;
using System.Collections.Generic;
using Rogue.Ingame.Data.Buff;

namespace Rogue.Ingame.Reward
{
    public class BuffChangeModel
    {
        public Action<BuffData> OnAddBuff;
        public Action<BuffData> OnRemoveBuff;

        private readonly OwnRewardModel ownReward;
        private readonly OwnSynergyModel ownSynergy;
        private List<BuffData> curBuffs = new List<BuffData>();
        private List<BuffData> tempBuffs = new List<BuffData>();

        public BuffChangeModel(OwnRewardModel ownReward, OwnSynergyModel ownSynergy)
        {
            this.ownReward = ownReward;
            this.ownSynergy = ownSynergy;

            ownSynergy.OnChanged += OnChanged;
            GetCurBuffs(ref curBuffs);
        }

        public void Reset()
        {
            curBuffs.Clear();
        }

        private void GetCurBuffs(ref List<BuffData> list)
        {
            list.Clear();
            foreach (var reward in ownReward.Rewards)
            {
                list.Add(reward.GetCurLevelBuff());
            }

            foreach (var synergy in ownSynergy.OwnSynergies)
            {
                if (synergy.TriggerLevel < 0)
                    continue;

                list.Add(synergy.Synergy.Rewards[synergy.TriggerLevel].Buff);
            }
        }

        private void OnChanged()
        {
            GetCurBuffs(ref tempBuffs);
            foreach (var buff in curBuffs)
            {
                if (!tempBuffs.Contains(buff))
                    OnRemoveBuff?.Invoke(buff);
            }
            foreach (var buff in tempBuffs)
            {
                if (!curBuffs.Contains(buff))
                    OnAddBuff?.Invoke(buff);
            }

            curBuffs.Clear();
            curBuffs.AddRange(tempBuffs);
            tempBuffs.Clear();
        }
    }
}