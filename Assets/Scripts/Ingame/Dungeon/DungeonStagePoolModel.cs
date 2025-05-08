using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.Random;
using Rogue.Ingame.Data;
using Rogue.Ingame.Stage;

namespace Rogue.Ingame.Dungeon
{
    public class DungeonStagePoolModel
    {
        private DungeonData dungeonData;
        private Dictionary<NodeType, Queue<string>> typeToQueue = new Dictionary<NodeType, Queue<string>>();
        private IRandom random;

        public DungeonStagePoolModel()
        {
        }

        public void Init(DungeonData dungeonData, int seed)
        {
            this.dungeonData = dungeonData;
            random = new RandomWithSeed(seed);
            typeToQueue.Clear();
            foreach (var stagePool in dungeonData.Stages)
            {
                var queue = new Queue<string>();
                typeToQueue.Add(stagePool.NodeType, queue);
                var pool = new List<string>(stagePool.Stages.Select(x => x.SceneName));
                while (pool.Count > 0)
                {
                    var idx = random.Range(0, pool.Count);
                    queue.Enqueue(pool[idx]);
                    pool.RemoveAt(idx);
                }
            }
        }

        public string Next(NodeType nodeType)
        {
            var value = typeToQueue[nodeType].Dequeue();
            typeToQueue[nodeType].Enqueue(value);
            return value;
        }
    }
}