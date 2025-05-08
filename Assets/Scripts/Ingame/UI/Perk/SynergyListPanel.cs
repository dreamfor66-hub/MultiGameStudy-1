using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Reward;
using Rogue.Ingame.Reward.Struct;
using Rogue.Ingame.Util.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI.Perk
{
    public class SynergyListPanel : MonoBehaviour
    {
        [SerializeField] [Required] private SynergySimpleView prefab;

        private UIPool<SynergySimpleView> pool;
        private readonly List<SynergySimpleView> spawned = new List<SynergySimpleView>();

        private OwnSynergyModel OwnSynergyModel => RewardModel.Instance.OwnSynergyModel;
        private SynergyChangePreviewModel SynergyPreviewModel => RewardModel.Instance.SynergyChangePreviewModel;

        public void Awake()
        {
            pool = new UIPool<SynergySimpleView>(prefab);
        }

        public void Start()
        {
            OwnSynergyModel.OnChanged += OnSynergyChanged;
            SynergyPreviewModel.OnChanged += OnSynergyChanged;
        }

        public void OnDestroy()
        {
            if (RewardModel.Instance != null)
            {
                OwnSynergyModel.OnChanged -= OnSynergyChanged;
                SynergyPreviewModel.OnChanged -= OnSynergyChanged;
            }
        }

        private void OnSynergyChanged()
        {
            Clear();
            Set(OwnSynergyModel.OwnSynergies);
        }

        private void Clear()
        {
            foreach (var obj in spawned)
            {
                pool.Return(obj);
            }
            spawned.Clear();
        }

        private void Set(IReadOnlyList<OwnSynergyInfo> synergies)
        {
            Clear();
            var idx = 0;
            foreach (var synergy in synergies)
            {
                if (synergy.Count == 0)
                    continue;
                var obj = pool.Get();
                obj.transform.SetSiblingIndex(idx++);
                spawned.Add(obj);
                if (SynergyPreviewModel.GainInfo.Any(x => x.Synergy == synergy.Synergy))
                {
                    obj.Set(SynergyPreviewModel.GainInfo.Find(x => x.Synergy == synergy.Synergy));
                }
                else if (SynergyPreviewModel.LoseInfo.Any(x => x.Synergy == synergy.Synergy))
                {
                    obj.Set(SynergyPreviewModel.LoseInfo.Find(x => x.Synergy == synergy.Synergy));
                }
                else
                {
                    obj.Set(synergy);
                }
            }
        }

        public void ShowDetail(bool show)
        {
            foreach (var obj in spawned)
                obj.ShowDetail(show);
        }
    }
}
