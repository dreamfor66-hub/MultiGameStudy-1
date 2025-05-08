using System;
using System.Collections.Generic;
using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Reward
{
    public enum RewardRarity
    {
        Common,
        UnCommon,
        Rare,
        Mystic
    }

    public enum SynergyTag
    {
        None = 0,
        Fire = 1,
        Ice,
        Spear,
        Heal,
        Wind,
        Drain,
        Plague,
        Spine,
    }

    [Serializable]
    public class RewardLevelData
    {
        public BuffData Buff;
        public string Title;
        [Multiline(3)]
        public string Description;
    }


    [CreateAssetMenu(fileName = "new RewardData", menuName = "Data/Reward")]
    public class RewardData : ScriptableObject
    {
        public string Title;
        [Multiline(3)] public string Description;
        [PreviewField] public Sprite Sprite;

        public BuffData[] LevelBuff;

        public RewardRarity Rarity;
        public List<SynergyTag> Tags;
        public bool NotStacked;
    }
}