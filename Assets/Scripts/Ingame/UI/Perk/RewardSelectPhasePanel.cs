using Rogue.Ingame.Reward;
using UnityEngine;

namespace Rogue.Ingame.UI.Perk
{
    public class RewardSelectPhasePanel : MonoBehaviour
    {
        private RewardPhaseModel RewardPhaseModel => RewardModel.Instance.RewardPhaseModel;

        private void Start()
        {
            RewardPhaseModel.OnChanged += OnPhaseChanged;
            OnPhaseChanged();
        }

        private void OnDestroy()
        {
            if (RewardModel.Instance != null)
            {
                RewardPhaseModel.OnChanged -= OnPhaseChanged;
            }
        }

        private void OnPhaseChanged()
        {
            if (RewardPhaseModel.Phase == RewardPhase.SelectReward)
                Show();
            else
                Hide();
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}