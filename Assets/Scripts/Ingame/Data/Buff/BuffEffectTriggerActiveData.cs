using System;
using System.Collections.Generic;
using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Rogue.Ingame.Buff
{
    public enum BuffActiveTargetType
    {
        Me,
        Target,
    }

    public enum BuffTargetTeamType
    {
        Attackable,
        Team,
        All,
    }

    [Flags]
    public enum BuffTargetEntityType
    {
        None = 0,
        Character = 1 << 0,
        Bullet = 1 << 1,
        All = 0xffff,
    }

    public enum BuffTargetRangeType
    {
        PivotOnly,
        Nearest,
        //Random,
        //Chain,
    }

    [Serializable]
    public class BuffTargetData
    {
        [FormerlySerializedAs("TargetType")]
        public BuffActiveTargetType ActiveTargetPivot = BuffActiveTargetType.Target;
        public BuffTargetRangeType RangeType = BuffTargetRangeType.PivotOnly;

        [Title("Details")]
        [ShowIf(nameof(ShowDetail))]
        public BuffTargetTeamType TeamType = BuffTargetTeamType.All;
        [ShowIf(nameof(ShowDetail))]
        public BuffTargetEntityType EntityType = BuffTargetEntityType.All;
        [ShowIf(nameof(ShowDetail))]
        public float MaxRange;
        [ShowIf(nameof(ShowDetail))]
        public int MaxNumber = 1;
        [ShowIf(nameof(ShowDetail))]
        public bool IncludePivot;

        private bool ShowDetail
        {
            get
            {
                return RangeType != BuffTargetRangeType.PivotOnly;
            }
        }
    }

    [Serializable]
    public class BuffEffectTriggerActiveData
    {
        public BuffTriggerData TriggerData;
        public BuffTargetData TargetData;
        public List<BuffActiveData> ActiveDataList;
    }
}