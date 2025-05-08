using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    public enum PatternPoolTag
    {
        Random = 0,
        Counter,
        Phase,
        Evade,

        Main1 = 101,
        Main2,
        Main3,
        Main4,

        Custom1 = 1000,
        Custom2,
        Custom3,
        Custom4,
        Custom5,
    }


    [Serializable]
    public class BotPhasePatternPoolData
    {
        public PatternPoolTag Tag;
        public int Chance = 1;
        public BotPatternData Pattern;
        public int WalkFrameMin = 0;
        public int WalkFrameMax = 180;

    }

    [Serializable]
    [HideLabel]
    public class BotPhaseCondition
    {
        [LabelText("Next Phase If Hp Less Than (0~1)")]
        public float HpLessThan;
    }

    [Serializable]
    public class BotSinglePhaseData
    {
        [HorizontalGroup("G1", LabelWidth = 150, MaxWidth = 0.3f)]
        public float CounterChance;
        [HorizontalGroup("G1")]
        public int CounterCooldownFrame;
        [HorizontalGroup("G1")]
        public int CounterByRage;
        [HorizontalGroup("G2", LabelWidth = 150, MaxWidth = 0.3f)]
        public float EvadeChance;
        [HorizontalGroup("G2")]
        public int EvadeCooldownFrame;
        public float WaitDecayByHurt;
        public bool UpdatePatternScoreOnStart;
        public BotPhaseCondition NextCondition;

        public List<PatternPoolTag> MainLoop;

        [TableList]
        public List<BotPhasePatternPoolData> PatternPool;

        public MaterialControlData MaterialData;
    }

    [CreateAssetMenu(fileName = "new BotPhaseData", menuName = "Data/Bot/Phase")]
    public class BotPhaseData : ScriptableObject
    {
        public List<BotSinglePhaseData> Phases;
    }
}