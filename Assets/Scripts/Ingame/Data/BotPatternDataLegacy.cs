using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    [Serializable]
    public class BotPhaseActionLegacy
    {
        [TableColumnWidth(55, Resizable = false)]
        public int Chance = 1;
        public ActionData Action;
        [TableColumnWidth(80, Resizable = false)]
        public float AttackRange = 3f;
        [TableColumnWidth(50, Resizable = false)]
        public int Repeat = 1;
        [TableColumnWidth(95, Resizable = false)]
        public int WaitFrameMin = 60;
        [TableColumnWidth(95, Resizable = false)]
        public int WaitFrameMax = 120;

        [HideLabel]
        public List<ActionData> PatternChain;
    }

    [Serializable]
    public class BotPhaseConditionLegacy
    {
        public float HpLessThan;
    }

    [Serializable]
    public class BotPhaseInfoLegacy
    {
        public float CounterChance;
        public int CounterCooldownFrame;
        [TableList] public List<BotPhaseActionLegacy> Actions;
        [TableList] public List<BotPhaseActionLegacy> CounterActions;
        [TableList] public List<BotPhaseActionLegacy> PhaseActions;

        public BotPhaseConditionLegacy NextCondition;
    }

    [CreateAssetMenu(fileName = "new BotPatternData", menuName = "Data/BotPattern Legacy")]
    public class BotPatternDataLegacy : ScriptableObject
    {
        public List<BotPhaseInfoLegacy> Phases;
    }
}