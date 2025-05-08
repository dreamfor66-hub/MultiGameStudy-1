using System.Collections.Generic;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data.Buff
{
    [System.Serializable]
    public class BuffUnitData
    {        
        [TableList]
        public List<BuffConditionData> Conditions;
        [TableList]
        public List<BuffEffectSimpleData> SimpleEffects;

        [TableList]
        public List<BuffEffectTriggerActiveData> TriggerActiveEffects;

        [TableList]
        public List<BuffEffectOverrideHitData> OverrideHitEffects;
    }
}