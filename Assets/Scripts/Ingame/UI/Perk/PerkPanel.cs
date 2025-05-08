using System;
using System.Text;
using System.Text.RegularExpressions;
using FMLib.UI.OnOff;
using Rogue.Ingame.Reward;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


namespace Rogue.Ingame.UI.Perk
{
    public class PerkPanel : MonoBehaviour
    {
        [SerializeField] [Required] private OnOffBehaviour onOff;
        [SerializeField] [Required] private Image iconImage;
        [SerializeField] [Required] private Text titleText;
        [SerializeField] [Required] private Text descText;

        public bool Enabled { get; private set; }

        [Button]
        public void Set(RewardData reward, int level, bool enable)
        {
            Enabled = enable;
            onOff.Set(enable);
            iconImage.sprite = reward.Sprite;
            titleText.text = reward.Title;
            descText.text = PerkDescParser.HighlightCurLevel(reward.Description, level);
        }


    }
}