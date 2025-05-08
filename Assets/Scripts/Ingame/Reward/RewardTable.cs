using System.Collections.Generic;
using Rogue.Ingame.Data.Buff;
using UnityEngine;

namespace Rogue.Ingame.Reward
{
    [CreateAssetMenu(fileName = "new RewardTable", menuName = "Data/Reward Table")]
    public class RewardTable : ScriptableObject
    {
        public List<RewardData> Rewards;
        public RewardData ReviveReward;

        public BuffData ReviveBuff;
        public BuffData Heal30Buff;

    }
}