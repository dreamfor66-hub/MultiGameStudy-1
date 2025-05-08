using System.Resources;
using Rogue.Ingame.Reward;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rogue.Ingame.UI.Perk
{
    public class RewardRemovePhasePanel : MonoBehaviour
    {
        [SerializeField] [Required] private PerkLevelPanel gainPerk;
        [SerializeField] [Required] private PerkLevelPanel removePerk;

        private RewardPhaseModel RewardPhaseModel => RewardModel.Instance.RewardPhaseModel;
        private SynergyChangePreviewModel synergyPreviewModel => RewardModel.Instance.SynergyChangePreviewModel;

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

        private GameObject lastSelected = null;

        public void Update()
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != lastSelected)
            {
                lastSelected = selected;
                var perkIcon = selected.GetComponent<PerkIconView>();
                if (perkIcon != null)
                {
                    synergyPreviewModel.SetLose(perkIcon.RewardLevel.Reward);
                    removePerk.Set(perkIcon.RewardLevel);
                }
            }
        }

        private void OnPhaseChanged()
        {
            if (RewardPhaseModel.Phase == RewardPhase.SelectRemove)
                Show();
            else
                Hide();
        }

        private void Show()
        {
            gainPerk.Set(RewardPhaseModel.SelectedReward);
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            lastSelected = null;
            gameObject.SetActive(false);
        }
    }
}
