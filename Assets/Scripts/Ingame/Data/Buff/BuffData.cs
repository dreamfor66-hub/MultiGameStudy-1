using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Rogue.Ingame.Data.Buff
{
    [CreateAssetMenu(fileName = "new BuffData", menuName = "Data/Buff")]
    public class BuffData : ScriptableObject
    {
        [FormerlySerializedAs("tag")]
        public List<BuffTag> Tags;
        public List<BuffUnitData> Buffs;
        public List<BuffReleaseCondition> ReleaseCondition; // or 연산
        public BuffOverlapMethod OverlapMethod;

        [Title("Showing")]
        public BuffVfxData VfxData;
        public BuffIconData IconData;
        public MaterialControlDataLegacy MaterialDataLegacy;
        public bool NeedSync => VfxData.Enabled || IconData.IsVisible || MaterialDataLegacy != null;
    }

    [System.Serializable]
    public struct BuffIconData
    {
        public bool IsVisible;
        public Sprite Icon;
    }
}
