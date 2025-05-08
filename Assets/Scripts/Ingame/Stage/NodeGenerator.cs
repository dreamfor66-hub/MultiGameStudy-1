using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.Random;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Rogue.Ingame.Stage
{
    [Serializable]
    public class NodeCountChance
    {
        public int Count;
        public int Chance;
    }

    [Serializable]
    [HideLabel]
    public class NodeMapGenData
    {
        public int Row;
        public int Column;

        [TableList]
        public List<NodeCountChance> NodeCountTable;

        [Tooltip("제거해도 되는 Edge 를 얼마나 남길지 여부. 0 = 최대한 제거, 100 = 최대한 연결")]
        public int DupEdgeRemainPercent;

        [TableList]
        public List<AddNodeTypeData> NodeTypeTable;
    }

    [Serializable]
    public class AddNodeTypeData
    {
        public NodeType NodeType;
        public int MinRow;
        public int MaxRow;
        public int Count;
        public int CantAppearWithIn;
    }

    public enum NodeType
    {
        Normal1 = 1,
        Normal2,
        Normal3,

        NormalGold = 11,

        MiddleBoss = 21,
        Boss = 31,

        Recovery = 101,

        Shop = 111,
    }

    public struct MapEdge
    {
        public Vector2Int From;
        public Vector2Int To;

        public MapEdge(Vector2Int from, Vector2Int to)
        {
            From = from;
            To = to;
        }

        public MapEdge(int fromX, int fromY, int toX, int toY)
        {
            From = new Vector2Int(fromX, fromY);
            To = new Vector2Int(toX, toY);
        }
    }

    public struct MapNode
    {
        public NodeType Type;
        public Vector2Int Pos;

        public MapNode(NodeType type, Vector2Int pos)
        {
            Type = type;
            Pos = pos;
        }
    }

    public class NodeMapData
    {
        public int ColumnCount;
        public int RowCount;
        public List<MapNode> Nodes = new List<MapNode>();
        public List<MapEdge> Edges = new List<MapEdge>();
    }

    public class NodeGenerator
    {
        private readonly NodeMapGenData data;
        private readonly bool[,] array;
        private readonly HashSet<MapEdge> edges;
        private readonly Dictionary<Vector2Int, NodeType> typeDictionary;
        private readonly IRandom random;

        public NodeGenerator(NodeMapGenData data, int seed)
        {
            this.data = data;
            array = new bool[data.Row, data.Column];
            edges = new HashSet<MapEdge>();
            typeDictionary = new Dictionary<Vector2Int, NodeType>();
            random = new RandomWithSeed(seed);
        }

        public NodeMapData Generate()
        {
            SelectNodes();
            CreateEdges();
            CreateTypes();
            return CreateMapData();
        }

        private void SelectNodes()
        {
            var start = data.Column >= 3 ? random.Range(1, data.Column - 1) : random.Range(0, data.Column);
            array[0, start] = true;

            for (var i = 0; i < data.Row - 2; i++)
            {
                var selectable = new HashSet<int>();
                var notLinked = new HashSet<int>();
                var count = data.NodeCountTable[RandomSelector.Select(random, data.NodeCountTable.Select(x => x.Chance))].Count;

                for (var j = 0; j < data.Column; j++)
                {
                    if (!array[i, j])
                        continue;
                    selectable.UnionWith(CanNext(j, data.Column));
                    notLinked.Add(j);
                }

                var selected = selectable.SelectN(random, count);
                foreach (var j in selected)
                {
                    array[i + 1, j] = true;
                    notLinked.RemoveWhere(x => Mathf.Abs(x - j) <= 1);
                }

                foreach (var j in notLinked)
                    array[i + 1, j] = true;
            }
            array[data.Row - 1, data.Column / 2] = true;
        }

        private void CreateEdges()
        {
            for (var i = 0; i < data.Row - 2; i++)
            {
                for (var j = 0; j < data.Column; j++)
                {
                    if (!array[i, j])
                        continue;
                    var cur = new Vector2Int(i, j);
                    var canNext = CanNext(j, data.Column);
                    foreach (var n in canNext)
                    {
                        if (!array[i + 1, n])
                            continue;
                        var next = new Vector2Int(i + 1, n);
                        if (j != n)
                        {
                            var cross = new MapEdge(i, n, i + 1, j);
                            if (edges.Contains(cross))
                            {
                                if (random.Range(0, 2) == 0)
                                    edges.Remove(cross);
                                else
                                    continue;
                            }
                        }
                        edges.Add(new MapEdge(cur, next));
                    }
                }
            }

            var curEdges = new List<MapEdge>(edges);
            foreach (var edge in curEdges)
            {
                if (GetNextEdges(edge.From).Count() > 1 && GetPrevEdges(edge.To).Count() > 1)
                {
                    if (data.DupEdgeRemainPercent < random.Range(0, 100))
                        edges.Remove(edge);
                }
            }

            for (var j = 0; j < data.Column; j++)
            {
                if (array[data.Row - 2, j])
                {
                    edges.Add(new MapEdge(data.Row - 2, j, data.Row - 1, data.Column / 2));
                }
            }
        }

        private void CreateTypes()
        {
            foreach (var typeData in data.NodeTypeTable)
            {
                var selectable = new HashSet<Vector2Int>();
                for (var i = typeData.MinRow; i <= typeData.MaxRow; i++)
                {
                    for (var j = 0; j < data.Column; j++)
                    {
                        var pos = new Vector2Int(i, j);
                        if (array[i, j] && !typeDictionary.ContainsKey(pos))
                            selectable.Add(pos);
                    }
                }

                var remainCount = typeData.Count;
                while (selectable.Count > 0 && remainCount > 0)
                {
                    var selected = selectable.SelectN(random, 1).First();
                    typeDictionary[selected] = typeData.NodeType;
                    var min = Mathf.Max(selected.x - typeData.CantAppearWithIn, typeData.MinRow);
                    var max = Mathf.Min(selected.x + typeData.CantAppearWithIn, typeData.MaxRow);
                    var nodes = FindConnectedNodes(selected, min, max);
                    selectable.ExceptWith(nodes);
                    remainCount--;
                }
            }
        }

        private IEnumerable<Vector2Int> FindConnectedNodes(Vector2Int pos, int minPos, int maxPos)
        {
            var queue = new Queue<Vector2Int>();
            var visited = new HashSet<Vector2Int>();

            visited.Add(pos);
            queue.Enqueue(pos);
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                if (cur.x < minPos || cur.x > maxPos)
                    continue;
                yield return cur;
                if (cur.x <= pos.x)
                {
                    GetPrevEdges(cur)
                        .Select(e => e.From)
                        .Where(x => !visited.Contains(x))
                        .ForEach((x) =>
                        {
                            visited.Add(x);
                            queue.Enqueue(x);
                        });
                }

                if (cur.x >= pos.x)
                {
                    GetNextEdges(cur)
                        .Select(e => e.To)
                        .Where(x => !visited.Contains(x))
                        .ForEach((x) =>
                        {
                            visited.Add(x);
                            queue.Enqueue(x);
                        });
                }
            }
        }

        private IEnumerable<MapEdge> GetPrevEdges(Vector2Int pos)
        {
            return (new int[] { pos.y - 1, pos.y, pos.y + 1 }).Select(y => new MapEdge(pos.x - 1, y, pos.x, pos.y)).Where(x => edges.Contains(x));
        }

        private IEnumerable<MapEdge> GetNextEdges(Vector2Int pos)
        {
            return (new int[] { pos.y - 1, pos.y, pos.y + 1 }).Select(y => new MapEdge(pos.x, pos.y, pos.x + 1, y)).Where(x => edges.Contains(x));
        }

        private NodeMapData CreateMapData()
        {
            var result = new NodeMapData();
            for (var i = 0; i < data.Row; i++)
            {
                for (var j = 0; j < data.Column; j++)
                {
                    if (array[i, j])
                    {
                        var pos = new Vector2Int(i, j);
                        var type = typeDictionary.ContainsKey(pos) ? typeDictionary[pos] : NodeType.Normal1;
                        result.Nodes.Add(new MapNode(type, new Vector2Int(i, j)));
                    }

                }
            }
            result.Edges.AddRange(edges);
            result.ColumnCount = data.Column;
            result.RowCount = data.Row;
            return result;
        }


        readonly List<int> tempList = new List<int>();
        private int[] CanNext(int j, int column)
        {
            tempList.Clear();
            if (j >= 1)
                tempList.Add(j - 1);
            tempList.Add(j);
            if (j < column - 1)
                tempList.Add(j + 1);
            return tempList.ToArray();
        }
    }
}
