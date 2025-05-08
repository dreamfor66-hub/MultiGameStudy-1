using System.Collections;
using System.Threading.Tasks;
using Photon.Bolt;
using Rogue.Ingame.Character;
using Rogue.Ingame.Dungeon;
using Rogue.Ingame.UI.Dungeon;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    public class BoltDungeonEventListener : GlobalEventListener
    {
        public IEnumerator Start()
        {
            while (DungeonModel.Instance == null)
                yield return null;
            DungeonModel.Instance.DungeonEventModel.OnSelected += OnSelected;
        }

        public void OnDestroy()
        {
            DungeonModel.Instance.DungeonEventModel.OnSelected -= OnSelected;
        }

        // 클라
        public override void OnEvent(NodeSelectStartEvent evnt)
        {
            var curPos = new Vector2Int(evnt.CurRow, evnt.CurColumn);
            if (OwnerCharacterHolder.OwnerCharacterExistAndAlive)
                DungeonModel.Instance.DungeonEventModel.StartSelect(curPos);
            else
                OnSelected(new Vector2Int(-1, 0));

        }

        // 클라
        public void OnSelected(Vector2Int pos)
        {
            var evt = NodeSelectedEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            evt.Row = pos.x;
            evt.Column = pos.y;
            evt.Send();
        }

        //클라
        public override async void OnEvent(NodeSelectEndEvent evnt)
        {
            var pos = new Vector2Int(evnt.Row, evnt.Column);
            SelectNodePanel.Instance.FinalSelect(pos);
            await Task.Delay(1000);
            DungeonModel.Instance.NodeMapModel.SelectNextNode(pos);
            DungeonModel.Instance.DungeonEventModel.Next();
        }

        public override void OnEvent(GameOverEvent evnt)
        {
            GameOverClearUI.Instance.ShowGameOver(evnt.TryCount);
        }

        public override void OnEvent(GameClearEvent evnt)
        {
            GameOverClearUI.Instance.ShowGameClear(evnt.TryCount);
        }

        public override void OnEvent(ResetDungeonEvent evnt)
        {
            DungeonModel.Instance.End();
            GameOverClearUI.Instance.HideAll();
        }

        // 서버
        public override void OnEvent(NodeSelectedEvent evnt)
        {
            var pos = new Vector2Int(evnt.Row, evnt.Column);
            if (pos.x < 0)
            {
                if (BoltNetwork.IsServer)
                {
                    BoltDungeonManager.Instance.NodeSkipped(evnt.RaisedBy);
                }
            }
            else
            {
                if (BoltNetwork.IsServer)
                {
                    BoltDungeonManager.Instance.NodeSelected(evnt.RaisedBy, pos);
                }

                if (!(BoltNetwork.IsServer && BoltServerCallbacks.IsDedicated))
                {
                    SelectNodePanel.Instance.AddSelection(pos);
                }
            }
        }
    }
}