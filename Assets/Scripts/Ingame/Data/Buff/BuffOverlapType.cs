using UnityEngine;

namespace Rogue.Ingame.Data.Buff
{
    public enum BuffOverlapType
    {
        Individual,
        RefreshTimeOnly,
        StackAndRefreshTime,
        IgnoreNew,
        ReplaceWithNew,
        SelectHigherLevelAndRefreshTime,
    }
}