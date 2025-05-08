using System;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Stage
{
    public class StageMain : MonoBehaviour
    {
        [Required] public StageRunner StageRunner;
        [Required] public StartPositions StartPositions;

        public bool IsEnd { get; private set; }

        private Action clearAction;

        public void Init(StageValueData stageValueData, NodeTypeBuffData nodeTypeBuffData, float playerCountBuff, Func<GameObject, Vector3, Quaternion, GameObject> spawnAction, Action clearAction)
        {
            StageRunner.Init(stageValueData, nodeTypeBuffData, playerCountBuff, spawnAction);
            this.clearAction = clearAction;
        }

        public void Start()
        {
            StageRunner.OnClear += Clear;
        }

        private void Clear()
        {
            BuffTriggerDispatcher.ClearStage();
            clearAction?.Invoke();
        }
    }
}