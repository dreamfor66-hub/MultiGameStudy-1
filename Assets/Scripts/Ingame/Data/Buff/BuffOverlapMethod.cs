using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Rogue.Ingame.Data.Buff
{
    [System.Serializable]
    [HideLabel]
    [Title("Overlap")]
    public struct BuffOverlapMethod
    {
        public BuffOverlapType OverlapType;

        public bool ShowStackCount => OverlapType == BuffOverlapType.StackAndRefreshTime;

        public bool ShowGroup => OverlapType == BuffOverlapType.SelectHigherLevelAndRefreshTime ||
                                 OverlapType == BuffOverlapType.ReplaceWithNew;

        [ShowIf(nameof(ShowStackCount))]
        public int MaxStackCount;

        [ShowIf(nameof(OverlapType), BuffOverlapType.SelectHigherLevelAndRefreshTime)]
        public int Level;

        [ShowIf(nameof(ShowGroup))]
        public BuffTag Tag;
    }
}