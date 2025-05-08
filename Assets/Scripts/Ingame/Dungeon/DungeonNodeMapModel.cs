using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Data;
using Rogue.Ingame.Stage;
using UnityEngine;

namespace Rogue.Ingame.Dungeon
{
    public class DungeonNodeMapModel
    {
        public NodeMapData MapData => mapData;
        public IReadOnlyList<MapNode> Passed => passed;


        private NodeMapData mapData;
        private readonly List<MapNode> passed = new List<MapNode>();

        public MapNode CurNode => passed[passed.Count - 1];
        public int CurIdx => CurNode.Pos.x;

        public DungeonNodeMapModel()
        {
        }

        public void Init(DungeonData dungeonData, int seed)
        {
            var nodeGenerator = new NodeGenerator(dungeonData.NodeGenData, seed);
            mapData = nodeGenerator.Generate();
            passed.Clear();
            passed.Add(FirstNode());
        }

        private MapNode FirstNode()
        {
            return mapData.Nodes.Find(x => x.Pos.x == 0);
        }

        public bool IsLastNode()
        {
            return CurIdx == mapData.RowCount - 1;
        }

        public IEnumerable<Vector2Int> SelectableNodes()
        {
            return mapData.Edges.Where(x => x.From == CurNode.Pos).Select(x => x.To);
        }

        public void SelectNextNode(Vector2Int pos)
        {
            var node = mapData.Nodes.Find(x => x.Pos == pos);
            passed.Add(node);
        }
    }
}