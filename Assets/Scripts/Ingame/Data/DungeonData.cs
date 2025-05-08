using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Stage;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    [Serializable]
    public class SceneReference
    {
        [HideInInspector]
        public string SceneName;
#if UNITY_EDITOR
        [HideLabel] [OnValueChanged(nameof(Changed))] public SceneAsset Scene;
        public void Changed()
        {
            SceneName = Scene != null ? Scene.name : "";
        }
#endif
    }

    [Serializable]
    public class StageValueData
    {
        public int AttackPercent;
        public int HpPercent;
    }

    [Serializable]
    public class NodeTypeBuffData
    {
        public Vector2Int EliteCountMinMax;
        public Vector2Int RandomBuffCountMinMax;
        [ValidateInput(nameof(Validate), "Buff Table 에 포함되지 않은 에셋이 존재합니다. 테이블을 업데이트 해주세요.")]
        public List<BuffData> EliteFixedBuffs;
        [ValidateInput(nameof(Validate), "Buff Table 에 포함되지 않은 에셋이 존재합니다. 테이블을 업데이트 해주세요.")]
        public List<BuffData> EliteRandomBuffs;

        private bool Validate(List<BuffData> list) => list.All(TableChecker.IsInTable);
    }

    [Serializable]
    public class StagePool
    {
        public NodeType NodeType;
        public List<SceneReference> Stages;
        public NodeTypeBuffData EliteData;
    }

    [CreateAssetMenu(fileName = "new DungeonData", menuName = "Data/Dungeon")]
    public class DungeonData : ScriptableObject
    {
        [Title("노드 생성")]
        public NodeMapGenData NodeGenData;

        [Space(50)]
        [Title("노드별 스테이지 풀")]
        public List<StagePool> Stages;

        [Space(50)]
        [Title("스테이지 진행 공/체 버프")]
        public List<StageValueData> Values;
    }
}