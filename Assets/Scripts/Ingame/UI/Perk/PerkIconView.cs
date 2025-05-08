using Rogue.Ingame.Reward;
using Rogue.Ingame.Stage;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Perk
{
    public class PerkIconView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private RarityColorView[] rarityColors;
        [SerializeField] private GameObject[] levelSlots;
        [SerializeField] private GameObject[] levelDots;

        public RewardLevel RewardLevel { get; private set; }

        private void Reset()
        {
            rarityColors = GetComponentsInChildren<RarityColorView>(true);
        }

        private void SetRarity(RewardRarity rarity)
        {
            foreach (var color in rarityColors)
                color.SetRarity(rarity);
        }

        private void SetLevel(int max, int cur)
        {
            if (max == 3)
            {
                levelSlots[0].SetActive(true);
                levelSlots[1].SetActive(true);
                levelSlots[2].SetActive(true);
                levelDots[0].SetActive(cur >= 0);
                levelDots[1].SetActive(cur >= 1);
                levelDots[2].SetActive(cur >= 2);
            }
            else if (max == 2)
            {
                levelSlots[0].SetActive(true);
                levelSlots[1].SetActive(false);
                levelSlots[2].SetActive(true);
                levelDots[0].SetActive(cur >= 0);
                levelDots[2].SetActive(cur >= 1);
            }
            else if (max == 1)
            {
                levelSlots[0].SetActive(false);
                levelSlots[1].SetActive(true);
                levelSlots[2].SetActive(false);
                levelDots[1].SetActive(cur >= 0);
            }
        }


        [Button]
        public void Set(RewardLevel rewardLevel)
        {
            this.RewardLevel = rewardLevel;
            iconImage.sprite = rewardLevel.Reward.Sprite;
            SetRarity(rewardLevel.Reward.Rarity);
            SetLevel(rewardLevel.Reward.LevelBuff.Length, rewardLevel.Level);
        }

        [Button]
        public void SetWithoutLevel(RewardData reward)
        {
            iconImage.sprite = reward.Sprite;
            SetRarity(reward.Rarity);
            SetLevel(0, 0);
        }
    }
}