using System.Threading.Tasks;
using JetBrains.Annotations;
using Rogue.Ingame.Reward;
using Rogue.Ingame.Stage;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rogue.Ingame.UI.Perk
{
    public class RewardPanel : MonoBehaviour
    {
        public bool IsShow => gameObject.activeSelf;

        [SerializeField] [Required] private SynergyListPanel synergyList;
        [SerializeField] [Required] private PerkSlotListPanel perkSlotList;
        [SerializeField] [Required] private PerkSelectListPanel perkSelectList;

        private SlotCountModel SlotCountModel => RewardModel.Instance.SlotCountModel;
        private SynergyChangePreviewModel SynergyPreviewModel => RewardModel.Instance.SynergyChangePreviewModel;
        private SelectRandomRewardModel SelectRandomRewardModel => RewardModel.Instance.SelectRandomRewardModel;
        private OwnRewardModel OwnRewardModel => RewardModel.Instance.OwnRewardModel;
        private RewardPhaseModel PhaseModel => RewardModel.Instance.RewardPhaseModel;

        //for Test
        [SerializeField] [Required] private SynergyTable synergyTable;
        [SerializeField] [Required] private RewardTable rewardTable;

        public void Show(int level)
        {
            PhaseModel.GoSelectPhase();
            SelectRandomRewardModel.Init(level);
            synergyList.ShowDetail(true);
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            lastSelected = null;
        }

        public void Hide()
        {
            PhaseModel.Clear();
            EventSystem.current.SetSelectedGameObject(null);
            lastSelected = null;
            synergyList.ShowDetail(false);
            gameObject.SetActive(false);
        }

        public void Remove(RewardLevel reward)
        {
            PhaseModel.GoRemovePhase(reward);
            var firstIcon = perkSlotList.First;
            if (firstIcon != null)
                ForceSelect(firstIcon.Button.gameObject);
            else
                ForceSelect(null);
        }

        private void ForceSelect(GameObject obj)
        {
            EventSystem.current.SetSelectedGameObject(obj);
            lastSelected = obj;
        }

        private GameObject lastSelected;

        public void Start()
        {
            Hide();
        }
        public void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (lastSelected != null)
                    EventSystem.current.SetSelectedGameObject(lastSelected);
                else if (perkSelectList.List.Count > 0)
                    EventSystem.current.SetSelectedGameObject(perkSelectList.List[0].gameObject);
            }
            else
            {
                var selected = EventSystem.current.currentSelectedGameObject;
                if (selected != lastSelected)
                {
                    lastSelected = selected;
                    if (PhaseModel.Phase == RewardPhase.SelectReward)
                        UpdateSelectedUI(selected);
                }
            }
        }

        public void Select(PerkLevelPanel perkPanel)
        {
            var reward = perkPanel.Reward;
            if (OwnRewardModel.NeedRemove(reward))
            {
                Remove(reward);
            }
            else
            {
                OwnRewardModel.AddOrUpdate(reward);
                SynergyPreviewModel.Clear();
                Hide();
            }
        }

        [UsedImplicitly]
        public void SelectRemove(PerkIconView perkIcon)
        {
            OwnRewardModel.Remove(perkIcon.RewardLevel);
            OwnRewardModel.AddOrUpdate(PhaseModel.SelectedReward);
            SynergyPreviewModel.Clear();
            Hide();
        }

        private void UpdateSelectedUI(GameObject selected)
        {
            if (selected == null)
            {
                SynergyPreviewModel.Clear();
                return;
            }
            var perk = selected.GetComponent<PerkLevelPanel>();
            if (perk != null)
                SynergyPreviewModel.SetGain(perk.Reward.Reward);
            else
                SynergyPreviewModel.Clear();

        }

        public void OnDisable()
        {
            lastSelected = null;
        }
    }
}