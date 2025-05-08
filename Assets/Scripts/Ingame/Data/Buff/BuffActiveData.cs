using System.Collections.Generic;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Bullet;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data.Buff
{
    public enum BuffActiveType
    {
        None = 0,
        BuffToTarget = 1,
        SpawnBullet = 2,
        InstantDeath = 3,
        JustDamage = 4,
        Heal = 5,
        ReviveRandomMemberHpPercent = 6,
        ForceAction = 7,
        AddStack = 8,
        AddShield = 10,
        ReviveTargetHpPercent = 11,
        Stun = 12,
        GainStatus = 13,
        BuffAddTime = 14,
        BuffAddStack = 15,
        BuffRelease = 16,
        SpawnMonster = 17,
        GainRootAggro = 18,

        HealPercent = 20,
    }

    public struct BuffIdentifer
    {
        public BuffTag Tag;
        public int SelfId;

        public BuffIdentifer(BuffTag tag, int selfId)
        {
            this.Tag = tag;
            this.SelfId = selfId;
        }
    }

    [System.Serializable]
    public class BuffSpawnBulletData
    {
        public int Frame;
        public float Angle;
        public float AngleY;
        public float Speed;
        public float RotationSpeed;
        public Vector3 Position;
        public Vector3 PositionRandomMin;
        public Vector3 PositionRandomMax;
        public ActionTypeMask ActionType;

        [ValidateInput("@TableChecker.IsInTable(BulletPrefab2)", "Bullet Table 에 포함되지 않은 에셋입니다. 테이블을 업데이트 해주세요.")]
        [AssetsOnly] public BulletBehaviour BulletPrefab2;
    }

    [System.Serializable]
    public class BuffActiveData
    {
        public BuffActiveType ActiveType;

        [ShowIf(nameof(ActiveType), BuffActiveType.BuffToTarget)]
        [ValidateInput("@TableChecker.IsInTable(Buff)", "Buff Table 에 포함되지 않은 에셋입니다. 테이블을 업데이트 해주세요.")]
        public BuffData Buff;

        [ShowIf(nameof(ActiveType), BuffActiveType.ForceAction)]
        public ActionData Action;

        [ShowIf(nameof(ActiveType), BuffActiveType.SpawnBullet)]
        public List<BuffSpawnBulletData> Bullets;

        [ShowIf(nameof(ActiveType), BuffActiveType.SpawnMonster)]
        public List<MonsterSpawnData> Monsters;

        [ShowIf(nameof(ActiveType), BuffActiveType.GainStatus)]
        public CharacterStatusType StatusType;

        [ShowIf(nameof(ShowTag))]
        public BuffTag Tag;

        [ShowIf(nameof(ActiveType), BuffActiveType.JustDamage)]
        public HitDamageType DamageType;

        [ShowIf(nameof(ShowValue))]
        public int Value;

        [ShowIf(nameof(ShowFrame))]
        public int Frame;

        [ShowIf(nameof(ActiveType), BuffActiveType.BuffAddTime)]
        public bool IsDurationChanged;

        [ShowIf(nameof(ShowIsStacked))]
        public bool IsStacked;

        public bool ShowValue
        {
            get
            {
                switch (ActiveType)
                {
                    case BuffActiveType.AddShield:
                    case BuffActiveType.AddStack:
                    case BuffActiveType.Heal:
                    case BuffActiveType.JustDamage:
                    case BuffActiveType.ReviveRandomMemberHpPercent:
                    case BuffActiveType.ReviveTargetHpPercent:
                    case BuffActiveType.BuffAddTime:
                    case BuffActiveType.BuffAddStack:
                    case BuffActiveType.HealPercent:
                    case BuffActiveType.GainRootAggro:
                        return true;
                    default:
                        return false;
                }

            }
        }

        public bool ShowFrame
        {
            get
            {
                switch (ActiveType)
                {
                    case BuffActiveType.AddShield:
                    case BuffActiveType.Stun:
                    case BuffActiveType.GainStatus:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool ShowIsStacked
        {
            get
            {
                switch (ActiveType)
                {
                    case BuffActiveType.JustDamage:
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
                switch (ActiveType)
                {
                    case BuffActiveType.BuffAddTime:
                    case BuffActiveType.BuffAddStack:
                    case BuffActiveType.BuffRelease:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}
