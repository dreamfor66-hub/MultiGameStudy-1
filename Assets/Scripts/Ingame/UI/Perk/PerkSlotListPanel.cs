using System.Collections.Generic;
using Photon.Bolt;
using Rogue.Ingame.Reward;
using Rogue.Ingame.Stage;
using Rogue.Ingame.Util.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI.Perk
{
    public class PerkSlotListPanel : MonoBehaviour
    {
        public PerkIconSlotView First => slots.Count > 0 ? slots[0] : null;

        [SerializeField] [Required] private PerkIconSlotView prefab;

        private UIPool<PerkIconSlotView> pool;
        private List<PerkIconSlotView> slots = new List<PerkIconSlotView>();

        private OwnRewardModel OwnRewardModel => RewardModel.Instance.OwnRewardModel;
        private SlotCountModel SlotCountModel => RewardModel.Instance.SlotCountModel;


        private void Awake()
        {
            pool = new UIPool<PerkIconSlotView>(prefab);
        }

        private void Start()
        {
            OwnRewardModel.OnChanged += OnChanged;
            SlotCountModel.OnChanged += OnChanged;
            OnChanged();
        }

        private void OnDestroy()
        {
            if (RewardModel.Instance != null)
            {
                OwnRewardModel.OnChanged -= OnChanged;
                SlotCountModel.OnChanged -= OnChanged;
            }
        }

        private void Clear()
        {
            foreach (var slot in slots)
            {
                pool.Return(slot);
            }
            slots.Clear();
        }

        private void OnChanged()
        {
            Set(OwnRewardModel.Rewards, SlotCountModel.SlotCount);
        }

        private void Set(IReadOnlyList<RewardLevel> own, int slotMax)
        {
            Clear();
            for (var i = 0; i < slotMax; i++)
            {
                var slot = pool.Get();
                slots.Add(slot);
                if (i < own.Count)
                    slot.Set(own[i]);
                else
                    slot.SetEmpty();
            }
        }

    }
}
