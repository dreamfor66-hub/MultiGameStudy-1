using FMLib.UI.OnOff;
using Rogue.Ingame.Reward;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Perk
{
    public class SynergyPanel : MonoBehaviour
    {
        [SerializeField] [Required] private OnOffBehaviour onOff;
        [SerializeField] [Required] private Image iconImage;
        [SerializeField] [Required] private Text nameText;
        [SerializeField] private Text[] descTexts;

        public bool Enabled { get; private set; }

        public void Set(SynergyData synergy, int count, bool enable)
        {
            iconImage.sprite = synergy.Sprite;
            nameText.text = synergy.Name;
            Enabled = enable;
            onOff.Set(enable);

            var idx = SynergyHelper.GetTriggerLevel(synergy, count);
            for (var i = 0; i < descTexts.Length; i++)
            {
                if (i >= synergy.Rewards.Count)
                    descTexts[i].gameObject.SetActive(false);
                else
                {
                    descTexts[i].gameObject.SetActive(true);
                    descTexts[i].text = SynergyHelper.GetString(synergy.Rewards[i]);
                    descTexts[i].color = (i == idx) ? Color.white : Color.gray;
                }
            }
        }
    }
}