using System;
using Rogue.Ingame.Buff;
using Sirenix.OdinInspector;

namespace Rogue.Ingame.Data.Buff
{
    [Flags]
    public enum ActionTypeMask
    {
        None = 0,
        Basic = 1 << 0,
        Special = 1 << 1,
        Ultimate = 1 << 2,
        Evade = 1 << 3,
        Elemental = 1 << 4,
        Auxiliary = 1 << 5,
        Skill = 1 << 6,

        //All except elemental type
        AllGeneral = 0xffef,
        All = 0xffff
    }

    public static class ActionTypeHelper
    {
        public static bool CheckType(ActionTypeMask mask, ActionTypeMask type)
        {
            return (mask & type) != 0;
        }
    }

    public enum BuffTriggerType
    {
        EveryValueFrame = 1,
        OnBuffStart = 3,
        AttackDo = 6,

        AttackHitTarget = 11,
        AttackHitTargetCritical = 12,

        ApplySpecificBuff = 31,
        BuffStackGreaterOrEqualThanValue = 32,


        Hurt = 40,
        SingleHurtMoreThanHpPercent = 41,
        CumulativeDamageMoreThanHpPercent = 42,

        Death = 50,

        ParryingSuccess = 60,

        KillTarget = 70,

        ClearStage = 80,

        ActionMoveMilliMeter = 90,
    }

    [System.Serializable]
    public class BuffTriggerData
    {
        public BuffTriggerType TriggerType;

        [ShowIf(nameof(ShowAttackType))]
        public ActionTypeMask AttackType = ActionTypeMask.All;
        [ShowIf(nameof(ShowTag))]
        public BuffTag Tag;
        [ShowIf(nameof(ShowValue))]
        public int Value;
        [ShowIf(nameof(ShowTarget))]
        public BuffTargetEntityType EntityType = BuffTargetEntityType.All;
        [ShowIf(nameof(ShowIndirectHit))]
        public bool TriggerOnIndirectHit = true;

        [Title("Common")]
        public int EveryNTimes;
        public float Chance = 100f;
        public int CooldownFrame;
        public int CooldownStage;


        public bool ShowAttackType
        {
            get
            {
                switch (TriggerType)
                {
                    case BuffTriggerType.AttackDo:
                    case BuffTriggerType.AttackHitTarget:
                    case BuffTriggerType.AttackHitTargetCritical:
                    case BuffTriggerType.ActionMoveMilliMeter:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool ShowTag
        {
            get
            {
                switch (TriggerType)
                {
                    case BuffTriggerType.ApplySpecificBuff:
                    case BuffTriggerType.BuffStackGreaterOrEqualThanValue:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool ShowValue
        {
            get
            {
                switch (TriggerType)
                {
                    case BuffTriggerType.ActionMoveMilliMeter:
                    case BuffTriggerType.CumulativeDamageMoreThanHpPercent:
                    case BuffTriggerType.EveryValueFrame:
                    case BuffTriggerType.SingleHurtMoreThanHpPercent:
                    case BuffTriggerType.BuffStackGreaterOrEqualThanValue:
                        return true;
                    default:
                        return false;
                }
            }

        }

        public bool ShowIndirectHit
        {
            get
            {
                switch (TriggerType)
                {
                    case BuffTriggerType.AttackHitTarget:
                    case BuffTriggerType.AttackHitTargetCritical:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool ShowTarget
        {
            get
            {
                switch (TriggerType)
                {
                    case BuffTriggerType.AttackHitTarget:
                    case BuffTriggerType.AttackHitTargetCritical:
                    case BuffTriggerType.KillTarget:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}