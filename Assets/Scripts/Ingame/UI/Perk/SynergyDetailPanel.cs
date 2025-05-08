using FMLib.UI.OnOff;
using Rogue.Ingame.Reward;
using Rogue.Ingame.Reward.Struct;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Perk
{
    public class SynergyDetailPanel : MonoBehaviour
    {
        [SerializeField] [Required] private Image iconImage;
        [SerializeField] [Required] private Text nameText;
        [SerializeField] [Required] private Text[] descTexts;
        [SerializeField] [Required] private OnOffBehaviour onOff;
        [SerializeField] private int id;

        private SynergyChangePreviewModel SynergyPreviewModel => RewardModel.Instance.SynergyChangePreviewModel;

        private void Start()
        {
            SynergyPreviewModel.OnChanged += OnPreviewChanged;
            OnPreviewChanged();
        }

        private void OnDestroy()
        {
            if (RewardModel.Instance != null)
            {
                SynergyPreviewModel.OnChanged -= OnPreviewChanged;
            }
        }

        public void OnPreviewChanged()
        {
            if (id >= SynergyPreviewModel.GainInfo.Count)
                Set(new SynergyChangeInfo());
            else
                Set(SynergyPreviewModel.GainInfo[id]);
        }

        public void Set(SynergyChangeInfo info)
        {
            if (info.Synergy == null)
                onOff.Off();
            else
            {
                onOff.On();
                iconImage.sprite = info.Synergy.Sprite;
                nameText.text = info.Synergy.Name;
                for (var i = 0; i < descTexts.Length; i++)
                {
                    if (i >= info.Synergy.Rewards.Count)
                        descTexts[i].gameObject.SetActive(false);
                    else
                    {
                        descTexts[i].gameObject.SetActive(true);
                        descTexts[i].text = SynergyHelper.GetString(info.Synergy.Rewards[i]);
                        descTexts[i].color = GetColor(i, info.CurLevel, info.NextLevel);
                    }
                }
            }
        }

        public void Set(SynergyData synergy, int count, int changeCount)
        {
            iconImage.sprite = synergy.Sprite;
            nameText.text = synergy.Name;

            var idx = SynergyHelper.GetTriggerLevel(synergy, count);
            var changedIdx = SynergyHelper.GetTriggerLevel(synergy, changeCount);

            for (var i = 0; i < descTexts.Length; i++)
            {
                if (i >= synergy.Rewards.Count)
                    descTexts[i].gameObject.SetActive(false);
                else
                {
                    descTexts[i].gameObject.SetActive(true);
                    descTexts[i].text = SynergyHelper.GetString(synergy.Rewards[i]);
                    descTexts[i].color = GetColor(i, idx, changedIdx);
                }
            }

            onOff.On();
        }

        private Color GetColor(int i, int curLevel, int nextLevel)
        {
            if (curLevel < nextLevel)
                if (i > curLevel && i <= nextLevel)
                    return Color.green;
            if (curLevel > nextLevel)
                if (i > nextLevel && i <= curLevel)
                    return Color.red;

            return (i <= curLevel) ? Color.white : Color.gray;
        }

        public void Hide()
        {
            onOff.Off();
        }
    }
}