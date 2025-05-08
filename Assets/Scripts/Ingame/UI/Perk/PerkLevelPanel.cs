using System;
using FMLib.UI.OnOff;
using Rogue.Ingame.Reward;
using Rogue.Ingame.Stage;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Perk
{
    public class PerkLevelPanel : MonoBehaviour
    {
        [SerializeField] [Required] private PerkIconView perkIcon;
        [SerializeField] [Required] private Text nameText;
        [SerializeField] [Required] private Text descText;

        [SerializeField] [Required] private PerkSynergyTagView synergyOnOff1;
        [SerializeField] [Required] private PerkSynergyTagView synergyOnOff2;

        public SynergyData Synergy1 { get; private set; }
        public SynergyData Synergy2 { get; private set; }
        public RewardLevel Reward { get; private set; }

        public void Set(RewardLevel rewardLevel)
        {
            Reward = rewardLevel;
            perkIcon.Set(rewardLevel);
            nameText.text = NameLevelStr(rewardLevel);
            descText.text = PerkDescParser.ShowOnlyCurLevel(rewardLevel.Reward.Description, rewardLevel.Level);

            switch (rewardLevel.Reward.Tags.Count)
            {
                case 1:
                    Synergy1 = RewardModel.Instance.SynergyTable.Synergies.Find(x => x.Tag == rewardLevel.Reward.Tags[0]);
                    Synergy2 = null;
                    synergyOnOff1.Set(Synergy1 != null, Synergy1);
                    synergyOnOff2.Set(Synergy2 != null, Synergy2);
                    break;
                case 2:
                    Synergy1 = RewardModel.Instance.SynergyTable.Synergies.Find(x => x.Tag == rewardLevel.Reward.Tags[0]);
                    Synergy2 = RewardModel.Instance.SynergyTable.Synergies.Find(x => x.Tag == rewardLevel.Reward.Tags[1]);
                    synergyOnOff1.Set(Synergy2 != null, Synergy2);
                    synergyOnOff2.Set(Synergy1 != null, Synergy1);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        private static string NameLevelStr(RewardLevel rewardLevel)
        {
            return $"{rewardLevel.Reward.Title} Lv.{rewardLevel.Level + 1}";
        }
    }
}