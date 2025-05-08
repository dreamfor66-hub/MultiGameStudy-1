using System;
using System.Collections.Generic;
using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Reward
{
    [Serializable]
    public class SynergyData
    {
        [PreviewField(height: 120)] [TableColumnWidth(120)]
        public Sprite Sprite;

        public string Name;
        public SynergyTag Tag;

        [TableList] public List<SynergyReward> Rewards;
    }

    [Serializable]
    public class SynergyReward
    {
        public int NeedCount;
        public BuffData Buff;
        public string Description;
    }


    [CreateAssetMenu(fileName = "new SynergyTable", menuName = "Data/Synergy Table")]
    public class SynergyTable : ScriptableObject
    {
        [TableList] public List<SynergyData> Synergies;
    }
}