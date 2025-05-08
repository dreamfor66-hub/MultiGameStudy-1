using System;
using System.Collections.Generic;
using System.Text;
using Rogue.Ingame.Reward;
using Rogue.Ingame.Stage;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI
{
    [Serializable]
    public struct RarityColor
    {
        public RewardRarity Rarity;
        public Color Color;
    }

    public class RewardView : MonoBehaviour
    {
        [SerializeField] [Required] private Text titleText;
        [SerializeField] [Required] private Text descText;
        [SerializeField] [Required] private Text tagText;
        [SerializeField] [Required] private Image rarityImage;
        [SerializeField] [Required] private GameObject levelUpObj;

        [SerializeField] private List<RarityColor> rarityColors;

        public RewardData Reward { get; private set; }

        [Button]
        public void Set(RewardLevel reward, bool showLevelUp)
        {
            this.Reward = reward.Reward;
            titleText.text = reward.Reward.Title;
            descText.text = reward.Reward.Description;

            tagText.gameObject.SetActive(!showLevelUp);
            levelUpObj.gameObject.SetActive(showLevelUp);
            tagText.text = TagsString(reward.Reward.Tags);

            var color = rarityColors.Find(x => x.Rarity == reward.Reward.Rarity).Color;
            rarityImage.color = color;
        }

        private static string TagsString(List<SynergyTag> tags)
        {
            var builder = new StringBuilder();
            foreach (var tag in tags)
            {
                builder.Append(RewardHelper.TagToString(tag));
                builder.Append(" ");
            }

            return builder.ToString();
        }
    }
}