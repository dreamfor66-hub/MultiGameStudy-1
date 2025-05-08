using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Reward.Struct;

namespace Rogue.Ingame.Reward
{
    public class OwnSynergyModel
    {
        public Action OnChanged;
        public IReadOnlyList<OwnSynergyInfo> OwnSynergies => ownSynergies;

        private readonly OwnRewardModel ownReward;
        private readonly SynergyTable synergyTable;

        private readonly List<OwnSynergyInfo> ownSynergies = new List<OwnSynergyInfo>();

        public OwnSynergyModel(OwnRewardModel ownReward, SynergyTable synergyTable)
        {
            this.ownReward = ownReward;
            this.synergyTable = synergyTable;
            ownReward.OnChanged += OnOwnRewardChanged;
            OnOwnRewardChanged();
        }

        public OwnSynergyInfo Find(SynergyTag tag)
        {
            return ownSynergies.Find(x => x.Synergy.Tag == tag);
        }

        private void OnOwnRewardChanged()
        {
            CalculateTagCounts(ownReward.Rewards.Select(x => x.Reward));
        }

        private void CalculateTagCounts(IEnumerable<RewardData> rewards)
        {
            ownSynergies.Clear();
            foreach (var synergy in synergyTable.Synergies)
            {
                var tag = synergy.Tag;
                var count = rewards.Sum(x => x.Tags.Sum(y => y == tag ? 1 : 0));
                ownSynergies.Add(new OwnSynergyInfo
                {
                    Count = count,
                    Synergy = synergy,
                    TriggerLevel = SynergyHelper.GetTriggerLevel(synergy, count),
                });
            }

            ownSynergies.Sort((a, b) => (b.TriggerLevel - a.TriggerLevel) * 100 + (b.Count - a.Count));
            OnChanged?.Invoke();
        }


    }
}