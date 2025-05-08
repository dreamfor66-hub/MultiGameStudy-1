using FMLib.UI.OnOff;
using Rogue.Ingame.Reward;
using Rogue.Ingame.Stage;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Perk
{
    public class PerkIconSlotView : MonoBehaviour
    {
        public Button Button => perkIconButton;

        [SerializeField] [Required] private PerkIconView perkIcon;
        [SerializeField] [Required] private OnOffBehaviour onOff;
        [SerializeField] [Required] private Button perkIconButton;

        private RewardPhaseModel RewardPhaseModel => RewardModel.Instance.RewardPhaseModel;

        private void Start()
        {
            RewardPhaseModel.OnChanged += OnPhaseChanged;
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
            perkIconButton.interactable = RewardPhaseModel.Phase == RewardPhase.SelectRemove;
        }

        public void Set(RewardLevel rewardLevel)
        {
            perkIcon.Set(rewardLevel);
            onOff.On();
        }


        public void SetEmpty()
        {
            onOff.Off();
        }
    }
}