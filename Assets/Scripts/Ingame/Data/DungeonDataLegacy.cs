using System;
using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    public enum StagePoolKey
    {
        Basic = 0,
        Boss = 100,
        MiddleBoss = 200,
    }

    [Serializable]
    public class StagePoolLegacy
    {
        public StagePoolKey Key => key;
        public List<string> StageNames => stageNames;

        [SerializeField] private StagePoolKey key;
        [SerializeField] [ReadOnly] private List<string> stageNames;
#if UNITY_EDITOR
        [OnCollectionChanged(nameof(Before), nameof(After))]
        [SerializeField]
        [OnValueChanged(nameof(After))]
        private List<SceneAsset> stages;

        public void Before()
        {
        }
        public void After()
        {
            Refresh();
        }
        [Button]
        public void Refresh()
        {
            stageNames = stages.Select(x => x.name).ToList();
        }

#endif
    }

    [Serializable]
    public class StageDifficultyDataLegacy
    {
        public StagePoolKey StageKey;
        public int AttackAddPercent;
        public int HpAddPercent;
        public Vector2Int EliteCountMinMax;
        public Vector2Int RandomBuffCountMinMax;
    }

    [Serializable]
    public class EliteBuffData
    {
        [ValidateInput(nameof(Validate), "Buff Table 에 포함되지 않은 에셋이 존재합니다. 테이블을 업데이트 해주세요.")]
        public List<BuffData> FixedBuffs;
        [ValidateInput(nameof(Validate), "Buff Table 에 포함되지 않은 에셋이 존재합니다. 테이블을 업데이트 해주세요.")]
        public List<BuffData> RandomBuffs;

        private bool Validate(List<BuffData> list) => list.All(TableChecker.IsInTable);
    }

    [CreateAssetMenu(fileName = "new DungeonLegacyData", menuName = "Data/Dungeon Legacy")]
    public class DungeonDataLegacy : ScriptableObject
    {
        [TableList] public List<StagePoolLegacy> Pools;
        [TableList] public List<StageDifficultyDataLegacy> Stages;
        [Title("Elite Buffs")] [HideLabel] public EliteBuffData Elite;
    }
}