using System;
using Rogue.Ingame.Reward;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Perk
{
    [RequireComponent(typeof(Graphic))]
    public class RarityColorView : MonoBehaviour
    {
        [SerializeField] private Graphic graphic;

        [SerializeField] private Color commonColor = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] private Color unCommonColor = new Color(0.9f, 0.9f, 0.1f);
        [SerializeField] private Color rareColor = new Color(0.1f, 0.4f, 0.9f);
        [SerializeField] private Color mysticColor = new Color(0.9f, 0.1f, 0.9f);

        void Reset()
        {
            graphic = GetComponent<Graphic>();
        }

        [Button]
        public void SetRarity(RewardRarity rarity)
        {
            graphic.color = GetColor(rarity);
        }

        public void SetGrade(int grade)
        {
            grade = Mathf.Clamp(grade, 0, 3);
            SetRarity((RewardRarity)grade);
        }

        private Color GetColor(RewardRarity rarity)
        {
            return rarity switch
            {
                RewardRarity.Common => commonColor,
                RewardRarity.UnCommon => unCommonColor,
                RewardRarity.Rare => rareColor,
                RewardRarity.Mystic => mysticColor,
                _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
            };
        }
    }
}