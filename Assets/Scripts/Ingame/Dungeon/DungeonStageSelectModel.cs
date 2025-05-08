using Rogue.Ingame.Data;

namespace Rogue.Ingame.Dungeon
{
    public class DungeonStageSelectModel
    {
        private readonly DungeonNodeMapModel nodeMap;
        private readonly DungeonStagePoolModel stagePool;
        public string CurSceneName { get; private set; }

        public DungeonStageSelectModel(DungeonNodeMapModel nodeMap, DungeonStagePoolModel stagePool)
        {
            this.nodeMap = nodeMap;
            this.stagePool = stagePool;
        }

        public void Init()
        {
            CurSceneName = stagePool.Next(nodeMap.CurNode.Type);
        }

    }
}