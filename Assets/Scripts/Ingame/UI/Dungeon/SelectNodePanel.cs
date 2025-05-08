using Rogue.Ingame.Debugger;
using Rogue.Ingame.Dungeon;
using Rogue.Ingame.Input;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI.Dungeon
{
    public class SelectNodePanel : MonoBehaviour
    {
        public static SelectNodePanel Instance { get; private set; }

        [SerializeField] [Required] private NodeMapDrawer nodeMapDrawer;
        [SerializeField] [Required] private GameObject panel;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            DungeonModel.Instance.OnInit += OnDungeonInit;
            DungeonModel.Instance.DungeonEventModel.OnStartSelect += OnStartSelect;
            DungeonModel.Instance.DungeonEventModel.OnNext += OnNext;
            nodeMapDrawer.OnSelected += OnSelected;
        }

        private void OnDestroy()
        {
            Instance = null;
            DungeonModel.Instance.OnInit -= OnDungeonInit;
            DungeonModel.Instance.DungeonEventModel.OnStartSelect -= OnStartSelect;
            DungeonModel.Instance.DungeonEventModel.OnNext -= OnNext;
            nodeMapDrawer.OnSelected -= OnSelected;
        }

        private void OnDungeonInit()
        {
            nodeMapDrawer.Init(DungeonModel.Instance.NodeMapModel.MapData);
        }

        private void OnStartSelect(Vector2Int pos)
        {
            var nodeMapModel = DungeonModel.Instance.NodeMapModel;
            nodeMapDrawer.ShowSelectUI(nodeMapModel.Passed);
            panel.SetActive(true);
            InputLock.Lock();
        }

        public void AddSelection(Vector2Int pos)
        {
            nodeMapDrawer.AddSelection(pos);
        }

        public void FinalSelect(Vector2Int pos)
        {
            nodeMapDrawer.FinalSelect(pos);
        }

        private void OnSelected(Vector2Int pos)
        {
            DungeonModel.Instance.DungeonEventModel.Selected(pos);
            nodeMapDrawer.DisableSelectables();
        }

        private void OnNext()
        {
            panel.SetActive(false);
            InputLock.UnLock();
        }
    }
}
