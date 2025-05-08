using System;
using Sirenix.OdinInspector;

namespace Rogue.Ingame.Data.Buff
{
    public enum BuffReleaseConditionType
    {
        Time = 0,
        //Death = 1,
        //Instant = 2,
        AttackDo = 3,
    }

    [Serializable]
    public struct BuffReleaseCondition
    {
        public BuffReleaseConditionType ReleaseConditionType;
        [ShowIf(nameof(ReleaseConditionType), BuffReleaseConditionType.Time)]
        public int TimeFrame;
    }
}