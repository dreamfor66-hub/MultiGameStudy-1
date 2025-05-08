using Rogue.Ingame.Reward;
using Sirenix.OdinInspector;
using System;
using System.Threading.Tasks;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Input;
using Rogue.Ingame.UI.Perk;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Stage
{
    [Serializable]
    public struct RarityChance
    {
        public RewardRarity Rarity;
        public int Chance;
    }

    public struct RewardLevel
    {
        public RewardData Reward;
        public int Level;

        public static RewardLevel Create(RewardData rewardData, int level)
        {
            return new RewardLevel
            {
                Reward = rewardData,
                Level = level,
            };
        }

        public BuffData GetCurLevelBuff() => Reward.LevelBuff[Level];
        public bool CanLevelUp => Level < Reward.LevelBuff.Length - 1;
        public int MaxLevel => Reward.LevelBuff.Length;
    }


    public class RewardManager : MonoBehaviour
    {
        public static RewardManager Instance { get; private set; }

        private void Start()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        [SerializeField] [Required] private SynergyTable synergyTable;
        [SerializeField] [Required] private RewardPanel rewardPanel;

        private bool IsDeadPlayer(IEntity entity)
        {
            if (entity is CharacterBehaviour character)
                return character.Team == Team.Player && character.IsDead;
            return false;
        }



        public async Task Reward(int level)
        {
            using (InputLock.Lock())
            {
                rewardPanel.Show(level);
                while (rewardPanel.IsShow)
                    await Task.Delay(200);
            }
            // using (InputLock.Lock())
            // {
            //     this.addAction = addAction;
            //     this.removeAction = removeAction;
            //     var rewardList = GetRandomRewards(RewardSelectCount);
            //     var givenList = givenRewards;
            //
            //     var needRevive = EntityTable.Entities.Any(IsDeadPlayer);
            //     if (needRevive)
            //     {
            //         var revive = RewardLevel.Create(reviveReward, 0);
            //         if (rewardList.Count < RewardSelectCount)
            //             rewardList.Add(revive);
            //         else
            //             rewardList[Random.Range(0, rewardList.Count)] = revive;
            //     }
            // }
        }


    }
}