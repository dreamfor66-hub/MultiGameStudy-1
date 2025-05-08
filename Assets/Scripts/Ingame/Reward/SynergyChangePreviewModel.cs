using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Reward.Struct;

namespace Rogue.Ingame.Reward
{
    public class SynergyChangePreviewModel
    {
        public Action OnChanged;

        private readonly OwnSynergyModel ownSynergyModel;
        private readonly OwnRewardModel ownRewardModel;

        public readonly List<SynergyChangeInfo> GainInfo;
        public readonly List<SynergyChangeInfo> LoseInfo;

        private readonly List<SynergyTagCount> gainCount;
        private readonly List<SynergyTagCount> loseCount;

        public SynergyChangePreviewModel(OwnSynergyModel ownSynergyModel, OwnRewardModel ownRewardModel)
        {
            this.ownSynergyModel = ownSynergyModel;
            this.ownRewardModel = ownRewardModel;

            GainInfo = new List<SynergyChangeInfo>();
            LoseInfo = new List<SynergyChangeInfo>();
            gainCount = new List<SynergyTagCount>();
            loseCount = new List<SynergyTagCount>();
            // 선택중에 Reward 가 추가되거나 하지는 않을거라고 가정하여 OwnSynergy 의 변화에 대응 안함.
        }

        public void SetGain(RewardData reward)
        {
            gainCount.Clear();
            foreach (var tag in reward.Tags)
            {
                if (ownRewardModel.Rewards.Any(x => x.Reward == reward))
                    gainCount.Add(new SynergyTagCount { Tag = tag, Count = 0 });
                else
                    gainCount.Add(new SynergyTagCount { Tag = tag, Count = 1 });
            }
            Calculate();
            OnChanged?.Invoke();
        }

        public void SetLose(RewardData reward)
        {
            loseCount.Clear();
            foreach (var tag in reward.Tags)
                loseCount.Add(new SynergyTagCount { Tag = tag, Count = -1 });
            Calculate();
            OnChanged?.Invoke();
        }

        public void Clear()
        {
            gainCount.Clear();
            loseCount.Clear();
            Calculate();
            OnChanged?.Invoke();
        }

        private void Calculate()
        {
            GainInfo.Clear();
            LoseInfo.Clear();

            var changes = new List<SynergyTagCount>();
            foreach (var change in gainCount)
            {
                if (changes.Any(x => x.Tag == change.Tag))
                {
                    var cur = gainCount.Find(x => x.Tag == change.Tag);
                    cur.Count += change.Count;
                }
                else
                    changes.Add(change);
            }

            foreach (var change in loseCount)
            {
                if (changes.Any(x => x.Tag == change.Tag))
                {
                    var cur = gainCount.Find(x => x.Tag == change.Tag);
                    cur.Count += change.Count;
                }
                else
                    changes.Add(change);
            }

            foreach (var change in changes)
            {
                if (change.Count >= 0)
                    GainInfo.Add(CreateChangeInfo(change.Tag, change.Count));
                else
                    LoseInfo.Add(CreateChangeInfo(change.Tag, change.Count));
            }
        }

        private SynergyChangeInfo CreateChangeInfo(SynergyTag tag, int changeCount)
        {
            var ownSynergy = ownSynergyModel.Find(tag);
            var info = new SynergyChangeInfo();
            var nextCount = ownSynergy.Count + changeCount;
            var nextLevel = SynergyHelper.GetTriggerLevel(ownSynergy.Synergy, nextCount);

            info.Synergy = ownSynergy.Synergy;
            info.CurCount = ownSynergy.Count;
            info.CurLevel = ownSynergy.TriggerLevel;
            info.NextCount = nextCount;
            info.NextLevel = nextLevel;
            return info;
        }
    }
}