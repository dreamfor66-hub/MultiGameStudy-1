using Rogue.Ingame.Data;
using Rogue.Ingame.Stage;
using UnityEngine;

namespace Rogue.Ingame.Dungeon
{
    public class DungeonDifficultyModel
    {
        private readonly DungeonData dungeonData;
        private readonly DungeonNodeMapModel nodeMapModel;

        public DungeonDifficultyModel(DungeonData dungeonData, DungeonNodeMapModel nodeMapModel)
        {
            this.dungeonData = dungeonData;
            this.nodeMapModel = nodeMapModel;
        }

        public NodeTypeBuffData NodeTypeBuff()
        {
            var nodeType = nodeMapModel.CurNode.Type;
            return dungeonData.Stages.Find(x => x.NodeType == nodeType).EliteData;
        }

        public StageValueData StageIdxBuff()
        {
            var idx = nodeMapModel.CurIdx;
            if (dungeonData.Values.Count == 0)
                return new StageValueData();
            else if (idx >= dungeonData.Values.Count)
                return dungeonData.Values[dungeonData.Values.Count - 1];
            else
                return dungeonData.Values[idx];
        }

        public float PlayerCountHpBuff(int playerCount)
        {
            var buffCount = Mathf.Clamp(playerCount - 1, 0, 2);
            return 1f + buffCount * .5f;
        }
    }
}