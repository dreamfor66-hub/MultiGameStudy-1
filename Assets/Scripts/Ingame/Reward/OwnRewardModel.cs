using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Stage;

namespace Rogue.Ingame.Reward
{
    public class OwnRewardModel
    {
        private readonly SlotCountModel slotCountModel;
        public Action OnChanged;
        public IReadOnlyList<RewardLevel> Rewards => rewards;

        private readonly List<RewardLevel> rewards;

        public OwnRewardModel(SlotCountModel slotCountModel)
        {
            this.slotCountModel = slotCountModel;
            rewards = new List<RewardLevel>();
        }

        public void AddOrUpdate(RewardLevel rewardLevel)
        {
            rewards.RemoveAll(x => x.Reward == rewardLevel.Reward);
            rewards.Add(rewardLevel);
            OnChanged?.Invoke();
        }

        public void Reset()
        {
            rewards.Clear();
            OnChanged?.Invoke();
        }

        public void Remove(RewardLevel rewardLevel)
        {
            rewards.Remove(rewardLevel);
            OnChanged?.Invoke();
        }

        public bool NeedRemove(RewardLevel rewardLevel)
        {
            if (rewards.Count < slotCountModel.SlotCount)
                return false;
            if (rewards.Any(x => x.Reward == rewardLevel.Reward))
                return false;
            return true;
        }
    }
}