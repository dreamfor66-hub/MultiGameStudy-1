using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.Random;
using Rogue.Ingame.Stage;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Reward
{
    public class SelectRandomRewardModel
    {
        public Action OnChanged;
        public IReadOnlyList<RewardLevel> Selectable => selectable;

        private readonly RewardTable rewardTable;
        private readonly OwnRewardModel ownRewards;

        private static readonly int selectCount = 3;

        private readonly List<RewardLevel> candidates = new List<RewardLevel>();
        private readonly List<RewardLevel> selectable = new List<RewardLevel>();
        private int curLevel = 0;

        private readonly RarityChance[] chanceTable =
        {
            new RarityChance {Rarity = RewardRarity.Common, Chance = 50},
            new RarityChance {Rarity = RewardRarity.UnCommon, Chance = 30},
            new RarityChance {Rarity = RewardRarity.Rare, Chance = 12},
            new RarityChance {Rarity = RewardRarity.Mystic, Chance = 8},
        };

        public SelectRandomRewardModel(RewardTable rewardTable, OwnRewardModel ownRewards)
        {
            this.rewardTable = rewardTable;
            this.ownRewards = ownRewards;
        }

        public void Init(int level)
        {
            curLevel = level;
            selectable.Clear();
            RefreshCandidates(level);
            for (var i = 0; i < selectCount; i++)
            {
                if (candidates.Count == 0)
                    break;

                var chances = chanceTable.Select(x => candidates.Any(y => y.Reward.Rarity == x.Rarity) ? x.Chance : 0);
                var rarity = chanceTable[RandomSelector.Select(RandomByUnity.Instance, chances)].Rarity;
                var cands = candidates.Where(x => x.Reward.Rarity == rarity).ToArray();
                var select = cands[Random.Range(0, cands.Length)];
                candidates.Remove(select);
                selectable.Add(select);
            }
            OnChanged?.Invoke();
        }

        public void ReRoll()
        {
            Init(curLevel);
        }

        private void RefreshCandidates(int level)
        {
            candidates.Clear();
            foreach (var reward in rewardTable.Rewards)
            {
                // if (level >= reward.LevelBuff.Length)
                //     continue;


                if (ownRewards.Rewards.Any(x => x.Reward == reward))
                {
                    var ownReward = ownRewards.Rewards.First(x => x.Reward == reward);
                    if (ownReward.CanLevelUp)
                        candidates.Add(RewardLevel.Create(reward, Mathf.Min(ownReward.Level + level + 1, ownReward.MaxLevel - 1)));
                }
                else
                {
                    candidates.Add(RewardLevel.Create(reward, Mathf.Min(level, reward.LevelBuff.Length - 1)));
                }
            }
        }
    }
}