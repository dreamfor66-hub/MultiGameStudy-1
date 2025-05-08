using JetBrains.Annotations;
using Rogue.Ingame.Goods;
using Rogue.Ingame.Reward;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Perk
{
    public class ReRollButton : MonoBehaviour
    {
        [SerializeField] [Required] private Text costText;

        private ReRollModel reRollModel => GoodsModel.Instance.ReRollModel;
        private SelectRandomRewardModel SelectRandomRewardModel => RewardModel.Instance.SelectRandomRewardModel;

        private void Start()
        {
            reRollModel.OnChanged += OnCostChanged;
            OnCostChanged();
        }

        private void OnDestroy()
        {
            if (RewardModel.Instance != null)
            {
                reRollModel.OnChanged -= OnCostChanged;
            }
        }

        private void OnCostChanged()
        {
            costText.text = reRollModel.Cost.ToString();
        }

        [UsedImplicitly]
        public void OnButtonClicked()
        {
            if (!reRollModel.CanReRoll)
                return;
            reRollModel.ReRoll();
            SelectRandomRewardModel.ReRoll();
        }
    }
}