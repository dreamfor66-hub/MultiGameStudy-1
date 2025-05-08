using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Attack.Struct
{
    public enum HitDamageType
    {
        Normal = 0,
        True = 1,
        Elemental = 2,
    }

    public struct HitMainInfo
    {
        public IEntity Attacker;
        public IEntity AttackerRoot;
        public IEntity Victim;
        public KnockbackInfo Knockback;
        public HitFxData HitFx;
        public Vector3 Position;
        public int HitstopReductionIdx;
        public bool SuperArmor;
        public bool IsDirect;
    }

    public struct HitDetailInfo
    {
        public ActionTypeMask ActionType;
        public HitDamageType DamageType;
        public bool IsInstantDeath;
        public bool IsDirect;
    }

    public struct HitInfo
    {
        public HitMainInfo Main;
        public HitDetailInfo Detail;
        public int BasicDamage;
        public int CriticalChance;
        public int AdditionalCriticalDamagePercent;
    }

    public struct HitResultInfo
    {
        public HitMainInfo Main;
        public HitDamageInfo Damage;
        public bool Killed;
    }

    public struct HitTotalInfo
    {
        public HitMainInfo Main;
        public HitDetailInfo Detail;
        public HitDamageInfo Damage;
    }

    public struct HitBuffInfo
    {
        public int AdditionalDamage;
        public int AdditionalDamagePercent;
        public int AdditionalTrueDamage;
        public int AdditionalElementalDamage;
        public int CriticalChance;
        public int AdditionalCriticalDamagePercent;
        public int VictimAdditionalDamagePercent;
    }

    public struct HitDamageInfo
    {
        public int FinalDamage;
        public bool IsCritical;
        public int DpDamage;
    }
}