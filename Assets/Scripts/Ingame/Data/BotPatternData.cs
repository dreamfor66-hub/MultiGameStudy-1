using System;
using System.Collections.Generic;
using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    public enum BotPatternTargetType
    {
        None,
        Aggro1 = 10,
        Aggro2,
        Aggro3,
        MinHpPercent = 20,
        MapCenter = 30,
        Boss = 40,
        PivotObject = 50,
    }

    [Serializable]
    public class BotPatternActionData
    {
        public ActionData Action;
        public BotPatternTargetType TargetType;
        public float MinRange = 20f;
        public int WaitFrame;
        public int TargetReduceAggro;
        public bool ResetAggroRank;
    }

    public enum BotPatternConditionType
    {
        None,
        HasBuff,
        NotHasBuff,
    }

    [Serializable]
    public class BotPatternConditionData
    {
        public BotPatternConditionType Type;
        public BuffTag BuffTag;
    }

    [CreateAssetMenu(fileName = "new BotPatternData", menuName = "Data/Bot/Pattern")]
    public class BotPatternData : ScriptableObject
    {
        public float ProperRange;
        public AnimationCurve RangeDeltaToChancePlus;

        public float CalibratedChance(float dist, float origChance)
        {
            if (RangeDeltaToChancePlus == null)
                return origChance;
            return Mathf.Clamp(RangeDeltaToChancePlus.Evaluate(dist - ProperRange) + origChance, 0f, float.MaxValue);
        }

        [TableList]
        public List<BotPatternConditionData> Conditions;

        [TableList]
        public List<BotPatternActionData> Actions;
    }
}