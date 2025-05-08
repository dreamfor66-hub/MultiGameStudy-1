using UnityEngine;

namespace Rogue.Ingame.Reward.Struct
{
    public struct SynergyTagCount
    {
        public SynergyTag Tag;
        public int Count;
    }

    public struct OwnSynergyInfo
    {
        public SynergyData Synergy;
        public int Count;
        public int TriggerLevel;
    }

    public struct SynergyChangeInfo
    {
        public SynergyData Synergy;
        public int CurCount;
        public int CurLevel;
        public int NextCount;
        public int NextLevel;
    }
}
