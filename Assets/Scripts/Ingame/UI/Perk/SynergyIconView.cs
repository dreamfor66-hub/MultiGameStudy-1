using Rogue.Ingame.Reward;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Perk
{
    public class SynergyIconView : MonoBehaviour
    {
        [SerializeField] [Required] private Image iconImage;
        [SerializeField] [Required] private RarityColorView rarityColor;

        public void Set(SynergyData synergy, int grade)
        {
            iconImage.sprite = synergy.Sprite;
            rarityColor.SetGrade(grade + 1);
        }

    }
}