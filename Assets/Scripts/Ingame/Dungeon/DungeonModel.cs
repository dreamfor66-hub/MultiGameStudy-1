using System;
using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Dungeon
{
    public class DungeonModel : MonoBehaviour
    {
        public DungeonData DungeonData;

        public static DungeonModel Instance { get; private set; }

        public DungeonEventModel DungeonEventModel;
        public DungeonNodeMapModel NodeMapModel;
        public DungeonStagePoolModel StagePoolModel;
        public DungeonStageSelectModel StageSelectModel;
        public DungeonDifficultyModel DungeonDifficultyModel;

        public Action OnInit;
        public bool IsInit { get; private set; }

        private void Awake()
        {
            Instance = this;
            DungeonEventModel = new DungeonEventModel();
            NodeMapModel = new DungeonNodeMapModel();
            StagePoolModel = new DungeonStagePoolModel();
            StageSelectModel = new DungeonStageSelectModel(NodeMapModel, StagePoolModel);
            DungeonDifficultyModel = new DungeonDifficultyModel(DungeonData, NodeMapModel);
        }

        private void OnDestroy()
        {
            Instance = this;
        }

        public void Init(int seed)
        {
            NodeMapModel.Init(DungeonData, seed);
            StagePoolModel.Init(DungeonData, seed);
            StageSelectModel.Init();
            OnInit?.Invoke();
            IsInit = true;
        }

        public void End()
        {
            IsInit = false;
        }
    }
}