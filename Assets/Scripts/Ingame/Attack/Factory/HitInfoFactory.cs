using FMLib.Extensions;
using FMLib.Structs;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Attack.Factory
{
    public static class HitInfoFactory
    {
        public static ParryingInfo CreateParrying(IEntity attacker, IEntity victim, HitboxInfo hitboxInfo)
        {
            ParryingInfo info;
            info.Attacker = attacker;
            info.Victim = victim;
            info.HitFx = hitboxInfo.HitFx;
            info.Direction = new VectorXZ(attacker.GameObject.transform.position - victim.GameObject.transform.position).Normalized;
            return info;
        }

        public static HitInfo Create(IEntity attackRoot, IEntity attackEntity, IEntity victimEntity,
            HitboxInfo hitboxInfo, Vector3 position, ActionTypeMask actionType, bool isDirectHit, int hitstopReductionIdx, bool superArmor, int criticalChance, int additionalCriticalDamagePercent)
        {
            HitInfo info;

            info.Main.Attacker = attackEntity;
            info.Main.AttackerRoot = attackRoot;
            info.Main.Victim = victimEntity;
            info.Main.Knockback = CreateKnockback(attackEntity.GameObject.transform, victimEntity.GameObject.transform, hitboxInfo.Knockback);
            info.Main.HitFx = hitboxInfo.HitFx;
            info.Main.Position = position;
            info.Main.HitstopReductionIdx = hitstopReductionIdx;
            info.Main.SuperArmor = superArmor;
            info.Main.IsDirect = isDirectHit;

            info.Detail.ActionType = actionType;
            info.Detail.DamageType = ActionToDamageType(actionType);
            info.Detail.IsInstantDeath = false;
            info.Detail.IsDirect = isDirectHit;

            info.BasicDamage = hitboxInfo.Damage;
            info.CriticalChance = criticalChance;
            info.AdditionalCriticalDamagePercent = additionalCriticalDamagePercent;
            return info;
        }

        public static HitInfo CreateInstantDeath(IEntity attackerRoot, IEntity victim)
        {
            HitInfo info;

            info.Main.Attacker = attackerRoot;
            info.Main.AttackerRoot = attackerRoot;
            info.Main.Victim = victim;
            info.Main.Knockback = JustDamage();
            info.Main.HitFx = null;
            info.Main.Position = Vector3.zero;
            info.Main.HitstopReductionIdx = 0;
            info.Main.SuperArmor = false;
            info.Main.IsDirect = false;

            info.Detail.ActionType = ActionTypeMask.None;
            info.Detail.DamageType = HitDamageType.True;
            info.Detail.IsInstantDeath = true;
            info.Detail.IsDirect = false;

            info.BasicDamage = 0;
            info.CriticalChance = 0;
            info.AdditionalCriticalDamagePercent = 0;
            return info;
        }

        public static HitInfo CreateJustDamage(HitDamageType type, int damage, IEntity target, IEntity attackerRoot)
        {
            HitInfo info;

            info.Main.Attacker = attackerRoot;
            info.Main.AttackerRoot = attackerRoot;
            info.Main.Victim = target;
            info.Main.Knockback = JustDamage();
            info.Main.HitFx = null;
            info.Main.Position = target.GameObject.transform.position + new Vector3(0f, 1.5f, 0f) + Random.insideUnitSphere;
            info.Main.HitstopReductionIdx = 0;
            info.Main.SuperArmor = false;
            info.Main.IsDirect = false;

            info.Detail.ActionType = DamagetoActionType(type);
            info.Detail.DamageType = type;
            info.Detail.IsInstantDeath = false;
            info.Detail.IsDirect = false;

            info.BasicDamage = damage;
            info.CriticalChance = 0;
            info.AdditionalCriticalDamagePercent = 0;
            return info;
        }

        private static KnockbackInfo JustDamage()
        {
            KnockbackInfo info;
            info.Strength = KnockbackStrength.JustDamage;
            info.Distance = 0;
            info.Direction = Vector3.zero;
            info.KnockStopFrame = 0;
            return info;
        }

        private static KnockbackInfo CreateKnockback(Transform attacker, Transform victim, KnockbackData knockbackData)
        {
            var tmDiff = (victim.position - attacker.position).ToVec3XZ().normalized;
            var attackerForward = attacker.forward;
            var ratio = knockbackData.TypeRatio;
            var inverseCheck = knockbackData.InverseKnockback ? -1 : 1;
            var angle = Vector3.SignedAngle(attackerForward, tmDiff, Vector3.up);
            var direction = Quaternion.AngleAxis(angle * ratio, Vector3.up) * attackerForward * inverseCheck;

            KnockbackInfo info;
            info.Strength = knockbackData.Strength;
            info.Distance = knockbackData.Distance;
            info.Direction = direction;
            info.KnockStopFrame = knockbackData.KnockStopFrame;
            return info;
        }

        private static HitDamageType ActionToDamageType(ActionTypeMask actionType)
        {
            return ActionTypeHelper.CheckType(ActionTypeMask.Elemental, actionType) ? HitDamageType.Elemental : HitDamageType.Normal;
        }
        private static ActionTypeMask DamagetoActionType(HitDamageType actionType)
        {
            return actionType == HitDamageType.Elemental ? ActionTypeMask.Elemental : ActionTypeMask.None; ;
        }
    }
}