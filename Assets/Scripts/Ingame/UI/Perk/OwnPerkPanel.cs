using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Input;
using Rogue.Ingame.Reward;
using Rogue.Ingame.Util.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Perk
{
    public class OwnPerkPanel : MonoBehaviour
    {
        [SerializeField] [Required] private GameObject mainPanel;
        [SerializeField] [Required] private SynergyPanel synergyPrefab;
        [SerializeField] [Required] private PerkPanel perkPrefab;
        [SerializeField] [Required] private ScrollRect scrollRect;

        private UIPool<SynergyPanel> synergyPool;
        private UIPool<PerkPanel> perkPool;


        private readonly List<SynergyPanel> synergyPanels = new List<SynergyPanel>();
        private List<PerkPanel> perkPanels = new List<PerkPanel>();

        private readonly SynergySort synergySort = new SynergySort();
        private RewardModel rewardModel => RewardModel.Instance;

        private bool isShow => mainPanel.activeSelf;
        private bool isShowAll = true;
        void Awake()
        {
            synergyPool = new UIPool<SynergyPanel>(synergyPrefab);
            perkPool = new UIPool<PerkPanel>(perkPrefab);
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (InputDetector.GetSelectLButton())
            {
                if (isShow)
                    Hide();
                else
                    Show();
            }

            if (isShow && InputDetector.GetOKButton())
            {
                if (isShowAll)
                    ShowOwnOnly();
                else
                    ShowAll();
            }
        }

        private void Clear()
        {
            foreach (var synergyPanel in synergyPanels)
            {
                synergyPool.Return(synergyPanel);
            }

            foreach (var perkPanel in perkPanels)
            {
                perkPool.Return(perkPanel);
            }

            synergyPanels.Clear();
            perkPanels.Clear();
        }

        public void Hide()
        {
            if (!isShow)
                return;
            Clear();
            mainPanel.SetActive(false);
            InputLock.UnLock();
        }

        public void Show()
        {
            if (isShow)
                return;
            SetData();
            ShowAll();
            mainPanel.SetActive(true);
            InputLock.Lock();
        }

        public void SetData()
        {
            Clear();

            var idx = 0;
            var synergies = rewardModel.OwnSynergyModel.OwnSynergies.ToList();
            synergies.Sort((a, b) => a.Count - b.Count);
            foreach (var synergy in rewardModel.OwnSynergyModel.OwnSynergies)
            {
                var synergyPanel = synergyPool.Get();
                var tagCount = synergy.Count;
                var activated = tagCount > 0;
                synergyPanel.Set(synergy.Synergy, tagCount, activated);
                synergyPanels.Add(synergyPanel);
                synergyPanel.transform.SetSiblingIndex(idx++);
            }
            idx = 0;
            var rewards = rewardModel.RewardTable.Rewards;
            rewards.Sort((a, b) => (a.Tags[0] == b.Tags[0]) ? b.Rarity - a.Rarity : a.Tags[0] - b.Tags[0]);
            foreach (var reward in rewards)
            {
                var perkPanel = perkPool.Get();
                var find = rewardModel.OwnRewardModel.Rewards.FirstOrDefault(x => x.Reward == reward);
                var level = -1;
                if (find.Reward == reward)
                    level = find.Level;

                perkPanel.Set(reward, level, level >= 0);
                perkPanels.Add(perkPanel);
                perkPanel.transform.SetSiblingIndex(idx++);
            }
        }

        [Button]
        public void ShowOwnOnly()
        {
            isShowAll = false;
            foreach (var synergy in synergyPanels)
                synergy.gameObject.SetActive(synergy.Enabled);
            foreach (var perk in perkPanels)
                perk.gameObject.SetActive(perk.Enabled);
        }

        [Button]
        public void ShowAll()
        {
            isShowAll = true;
            foreach (var synergy in synergyPanels)
                synergy.gameObject.SetActive(true);
            foreach (var perk in perkPanels)
                perk.gameObject.SetActive(true);
        }
    }
}