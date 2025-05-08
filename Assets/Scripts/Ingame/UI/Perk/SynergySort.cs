using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Reward;

namespace Rogue.Ingame.UI.Perk
{
    public class SynergySort
    {
        private readonly Dictionary<SynergyTag, int> tagCountDict = new Dictionary<SynergyTag, int>();

        public void Reset(IEnumerable<RewardData> ownRewards)
        {
            tagCountDict.Clear();

            foreach (SynergyTag tag in Enum.GetValues(typeof(SynergyTag)))
            {
                var tagCount = ownRewards.Sum(x => x.Tags.Sum(y => y == tag ? 1 : 0));
                tagCountDict.Add(tag, tagCount);
            }
        }


        public List<SynergyData> SortSynergy(List<SynergyData> synergies)
        {
            var ret = new List<SynergyData>(synergies);
            ret.Sort((a, b) =>
            {
                var tagA = OwnCount(a);
                var tagB = OwnCount(b);
                var triggerA = GetTriggerCount(a, tagA);
                var triggerB = GetTriggerCount(b, tagB);
                if (triggerA != triggerB)
                    return triggerB - triggerA;
                else
                    return tagB - tagA;

            });
            return ret;
        }

        private int GetTriggerCount(SynergyData synergy, int ownCount)
        {
            var ret = 0;
            foreach (var reward in synergy.Rewards)
            {
                if (reward.NeedCount <= ownCount)
                    ret = reward.NeedCount;
            }

            return ret;
        }

        public int OwnCount(SynergyData synergy)
        {
            return tagCountDict[synergy.Tag];
        }

        public bool IsActivated(SynergyData synergy)
        {
            return OwnCount(synergy) > 0;
        }

    }
}