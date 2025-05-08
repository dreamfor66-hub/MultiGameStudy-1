using System.Collections.Generic;
using Rogue.Ingame.Reward;
using Rogue.Ingame.Stage;
using Rogue.Ingame.Util.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI.Perk
{
    public class PerkSelectListPanel : MonoBehaviour
    {
        public List<PerkLevelPanel> List => spawned;

        [SerializeField] [Required] private PerkLevelPanel prefab;

        private UIPool<PerkLevelPanel> pool;
        private readonly List<PerkLevelPanel> spawned = new List<PerkLevelPanel>();

        private SelectRandomRewardModel SelectRandomRewardModel => RewardModel.Instance.SelectRandomRewardModel;

        public void Awake()
        {
            pool = new UIPool<PerkLevelPanel>(prefab);
        }

        private void Start()
        {
            SelectRandomRewardModel.OnChanged += OnSelectedChanged;
            OnSelectedChanged();
        }

        private void OnDestroy()
        {
            if (RewardModel.Instance != null)
            {
                SelectRandomRewardModel.OnChanged -= OnSelectedChanged;
            }
        }

        private void OnSelectedChanged()
        {
            Set(SelectRandomRewardModel.Selectable);
        }

        private void Clear()
        {
            foreach (var obj in spawned)
            {
                pool.Return(obj);
            }
            spawned.Clear();
        }

        private void Set(IEnumerable<RewardLevel> rewardLevels)
        {
            Clear();
            foreach (var reward in rewardLevels)
            {
                var obj = pool.Get();
                spawned.Add(obj);
                obj.Set(reward);

            }
        }
    }
}