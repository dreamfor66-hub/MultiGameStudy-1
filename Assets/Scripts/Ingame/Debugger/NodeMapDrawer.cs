using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Stage;
using Rogue.Ingame.UI;
using Rogue.Ingame.Util.Pool;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Debugger
{
    public class NodeMapDrawer : MonoBehaviour
    {
        [SerializeField] [Required] private UINavigationHelper navigationHelper;
        public RectTransform nodeSample;
        public RectTransform edgeSample;
        public RectTransform thisRect;
        public Action<Vector2Int> OnSelected;

        public float nodeSize = 72f;
        public float topMargin = 80f;
        public float bottomMargin = 80f;
        public float horInterval = 120f;
        public float verInterval = 120f;
        public float edgeMargin = 20f;
        public float verRandom = 10f;
        public float horRandom = 10f;


        [ShowInInspector]
        private NodeMapData mapData;
        private UIPool<RectTransform> nodePool;
        private UIPool<RectTransform> edgePool;
        private readonly Dictionary<Vector2Int, Vector2> posDict = new Dictionary<Vector2Int, Vector2>();
        private readonly Dictionary<Vector2Int, NodeIcon2> nodeDict = new Dictionary<Vector2Int, NodeIcon2>();

        public void Init(NodeMapData data)
        {
            if (nodePool == null)
            {
                nodePool = new UIPool<RectTransform>(nodeSample);
                edgePool = new UIPool<RectTransform>(edgeSample);
            }

            mapData = data;
            nodePool.Clear();
            edgePool.Clear();
            posDict.Clear();
            nodeDict.Clear();

            foreach (var node in data.Nodes)
            {
                var pos = Position(node.Pos.x, node.Pos.y, data.ColumnCount);
                pos += new Vector2(Random.Range(-horRandom, horRandom), Random.Range(-verRandom, verRandom));
                posDict.Add(node.Pos, pos);
            }

            foreach (var edge in data.Edges)
            {
                var fromPos = posDict[edge.From] + new Vector2(0, -nodeSize / 4f);
                var toPos = posDict[edge.To] + new Vector2(0, nodeSize / 4f);
                var middle = (fromPos + toPos) / 2f;
                var angle = -Mathf.Atan((toPos.x - fromPos.x) / (toPos.y - fromPos.y)) * Mathf.Rad2Deg;
                var length = Vector2.Distance(fromPos, toPos) * .75f;

                var edgeObj = edgePool.Get();
                edgeObj.anchoredPosition = middle;
                edgeObj.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                edgeObj.sizeDelta = new Vector2(3f, length);
            }

            foreach (var node in data.Nodes)
            {
                var nodeObj = nodePool.Get();
                nodeObj.anchoredPosition = posDict[node.Pos];
                var nodeIcon = nodeObj.GetComponent<NodeIcon2>();
                nodeIcon.Set(node.Type);
                nodeIcon.Button.onClick.RemoveAllListeners();
                nodeIcon.Button.onClick.AddListener(() => OnClick(node.Pos));
                nodeDict.Add(node.Pos, nodeIcon);
            }

            var height = topMargin + bottomMargin + verInterval * (data.RowCount - 1) + nodeSize;
            thisRect.sizeDelta = new Vector2(thisRect.sizeDelta.x, height);
        }

        public void OnClick(Vector2Int pos)
        {
            OnSelected?.Invoke(pos);
        }

        [Button]
        public void ShowSelectUI(IReadOnlyList<MapNode> passed)
        {
            var curNode = passed.Last();
            var canNext = mapData.Edges.Where(x => x.From == curNode.Pos).Select(x => x.To);
            foreach (var pos in nodeDict.Keys)
            {
                var node = nodeDict[pos];
                node.ClearSelection();
                if (passed.Any(x => x.Pos == pos))
                {
                    node.SetButtonActive(false);
                    node.SetCheck(true);
                    node.SetEnable(true);
                }
                else if (canNext.Contains(pos))
                {
                    node.SetButtonActive(true);
                    node.SetCheck(false);
                    node.SetEnable(true);
                }
                else
                {
                    node.SetButtonActive(false);
                    node.SetCheck(false);
                    node.SetEnable(false);
                }
            }
            navigationHelper.SetSelectable(canNext.Select(x => nodeDict[x].gameObject));
        }

        public void DisableSelectables()
        {
            nodeDict.Values.ForEach(x => x.SetButtonActive(false));
        }

        private Vector2 Position(int i, int j, int column)
        {
            var x = (j - (column - 1) / 2f) * horInterval;
            var y = -(topMargin + nodeSize / 2f + verInterval * i);
            return new Vector2(x, y);
        }

        public void AddSelection(Vector2Int pos)
        {
            nodeDict[pos].AddSelection();
        }

        public void FinalSelect(Vector2Int pos)
        {
            nodeDict[pos].SetCheck(true);
        }
    }
}