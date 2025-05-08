using UnityEngine;

namespace Rogue.Ingame.Reward
{
    public class RewardModel : MonoBehaviour
    {
        public static RewardModel Instance { get; private set; }

        public SynergyTable SynergyTable;
        public RewardTable RewardTable;

        public SlotCountModel SlotCountModel;
        public RewardPhaseModel RewardPhaseModel;
        public OwnRewardModel OwnRewardModel;
        public OwnSynergyModel OwnSynergyModel;
        public SynergyChangePreviewModel SynergyChangePreviewModel;
        public SelectRandomRewardModel SelectRandomRewardModel;
        public BuffChangeModel BuffChangeModel;

        private void Awake()
        {
            Instance = this;
            Init();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void Init()
        {
            SlotCountModel = new SlotCountModel();
            RewardPhaseModel = new RewardPhaseModel();
            OwnRewardModel = new OwnRewardModel(SlotCountModel);
            OwnSynergyModel = new OwnSynergyModel(OwnRewardModel, SynergyTable);
            SynergyChangePreviewModel = new SynergyChangePreviewModel(OwnSynergyModel, OwnRewardModel);
            SelectRandomRewardModel = new SelectRandomRewardModel(RewardTable, OwnRewardModel);
            BuffChangeModel = new BuffChangeModel(OwnRewardModel, OwnSynergyModel);
        }

        public void Reset()
        {
            BuffChangeModel.Reset();
            SlotCountModel.Reset();
            RewardPhaseModel.Clear();
            OwnRewardModel.Reset();
        }
    }
}
