using System.Collections.Generic;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Stage
{
    public class DungeonGenerator
    {
        private Dictionary<StagePoolKey, Queue<string>> keyToQueue = new Dictionary<StagePoolKey, Queue<string>>();

        public DungeonGenerator(List<StagePoolLegacy> pools)
        {
            EnqueueAll(pools);
        }

        private void EnqueueAll(List<StagePoolLegacy> pools)
        {
            foreach (var pool in pools)
            {
                keyToQueue.Add(pool.Key, new Queue<string>());
                var stages = new List<string>(pool.StageNames);
                while (stages.Count > 0)
                {
                    var idx = Random.Range(0, stages.Count);
                    keyToQueue[pool.Key].Enqueue(stages[idx]);
                    stages.RemoveAt(idx);
                }
            }
        }


        public string Next(StagePoolKey key)
        {
            var value = keyToQueue[key].Dequeue();
            keyToQueue[key].Enqueue(value);
            return value;
        }

        public void GetListNonAlloc(IEnumerable<StagePoolKey> keys, ref List<string> result)
        {
            result.Clear();
            foreach (var key in keys)
            {
                result.Add(Next(key));
            }
        }
    }
}