using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.Structs;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    public enum KnockbackStrength
    {
        Low,
        Mid,
        High,

        JustDamage = 15,
    }

    [System.Serializable]
    public class ActionMoveData
    {
        public int StartFrame;
        public int EndFrame;
        public float FixedDirMoveSpeed;
        public float FixedRotSpeed;
        public float FollowTargetSpeed;

        public bool LookingTarget;
    }

    [System.Serializable]
    public class KnockbackData
    {
        public KnockbackStrength Strength;

        [DisableIf(nameof(Strength), KnockbackStrength.JustDamage)]
        public float Distance;

        [EnableIf(nameof(Strength), KnockbackStrength.Low)]
        public int KnockStopFrame;

        [Tooltip("0 일 경우 공격자의 Forward 방향, 1일경우 Transform Diff 방향.")]
        [Range(0, 1)]
        [DisableIf(nameof(Strength), KnockbackStrength.JustDamage)]
        public float TypeRatio;
        public bool InverseKnockback = false;

        [ShowInInspector]
        public float KnockbackFrame => KnockbackCalculator.TotalTime(Distance) * CommonVariables.GameFrame;
    }

    [System.Serializable]
    public class CharacterStatusData
    {
        public CharacterStatusType Type;
        public int StartFrame;
        public int EndFrame;
    }

    [System.Serializable]
    public class ActionBulletPatternData
    {
        public int StartFrame;
        [ValidateInput("@TableChecker.IsInTable(BulletPrefab2)", "Bullet Table 에 포함되지 않은 에셋입니다. 테이블을 업데이트 해주세요.")]
        public BulletBehaviour BulletPrefab2;
        public BulletPatternData PatternData;
        public BulletRandomizedPatternData RandomizedPatternData;
    }

    [Flags]
    public enum PositionAnchorUsage
    {
        None = 0,
        Position = 1,
        AngleXZ = 2,
        AngleY = 4,
        All = 7,
        TargetCharacter = 8,
        World = 16,
    }




    [System.Serializable]
    public class ActionBulletSpawnData
    {
        public int Frame;
        [ValidateInput("@TableChecker.IsInTable(BulletPrefab2)", "Bullet Table 에 포함되지 않은 에셋입니다. 테이블을 업데이트 해주세요.")]
        public BulletBehaviour BulletPrefab2;
        public float Speed;

        public PositionAnchorUsage AnchorUsage;

        [DisableIf(nameof(AnchorUsage), PositionAnchorUsage.None)]
        public int AnchorId;

        [DisableIf("@((int)this.AnchorUsage & (int)PositionAnchorUsage.Position) > 0")]
        public Vector3 Position;

        [DisableIf("@((int)this.AnchorUsage & (int)PositionAnchorUsage.Position) > 0")]
        public VectorXZ RandomPosition;

        [DisableIf("@((int)this.AnchorUsage & (int)PositionAnchorUsage.AngleXZ) > 0")]
        public float Angle;

        [DisableIf("@((int)this.AnchorUsage & (int)PositionAnchorUsage.AngleY) > 0")]
        public float AngleYMin;

        [DisableIf("@((int)this.AnchorUsage & (int)PositionAnchorUsage.AngleY) > 0")]
        public float AngleYMax;
    }

    [System.Serializable]
    public class ActionBuffData
    {
        public int Frame;

        [ValidateInput("@TableChecker.IsInTable(Buff)", "Buff Table 에 포함되지 않은 에셋입니다. 테이블을 업데이트 해주세요.")]
        public BuffData Buff;
    }

    [System.Serializable]
    public class ActionStateData
    {
        public string Key;
        public int StartFrame;
        public int EndFrame;
    }

    [CreateAssetMenu(fileName = "new ActionData", menuName = "Data/Action")]
    public class ActionData : ScriptableObject
    {
        [Title("Common")]
        public string ActionKey;
        public int TotalFrame;

        [Title("Animation")]
        public int AnimationOriginalFrame;
        public AnimationCurve AnimationAdjustCurve;

        [Title("Move")]
        public AnimationCurve MoveCurve = new AnimationCurve();
        public List<ActionMoveData> MoveData = new List<ActionMoveData>();

        public AnimationCurve MoveCurveX = new AnimationCurve();
        public AnimationCurve MoveCurveY = new AnimationCurve();

        [Title("Attack")]
        public List<AttackHitboxData> AttackHitboxData = new List<AttackHitboxData>();

        [Title("Bullet")]
        [HideLabel]
        public ActionBulletPatternData BulletPatternData;

        [TableList]
        public List<ActionBulletSpawnData> BulletSpawnData;


        [Title("Vfx")]
        public List<ActionVfxData> VfxData = new List<ActionVfxData>();

        [Title("Status")]
        public List<CharacterStatusData> StatusData = new List<CharacterStatusData>();

        [Title("MonsterSpawn")]
        public List<MonsterSpawnData> MonsterSpawnData = new List<MonsterSpawnData>();

        [Tooltip("특정 프레임에 나에게 거는 버프")]
        [Title("Buff")] public List<ActionBuffData> BuffData = new List<ActionBuffData>();


        [Title("State")]
        public int ExitableFrame;
        public List<ActionStateData> CustomStates;

        [Title("AttackType")]
        public ActionTypeMask AttackType;

        [Title("Auto Correction")]
        public bool EnableAutoCorrection;

        [ShowIf(nameof(EnableAutoCorrection))]
        public float AutoCorrectionMaxDistance;
        [ShowIf(nameof(EnableAutoCorrection))]
        public float AutoCorrectionDefaultDistance = 120f;
        [ShowIf(nameof(EnableAutoCorrection))]
        public float AutoCorrectionMaxAngle = 40f;
        [ShowIf(nameof(EnableAutoCorrection))]
        public float AutoCorrectionMinAngle = 2f;


        [Title("Resource")]
        public int StaminaUse;
        public int StackUse;
        public int CoolTimeFrame;

        [Title("Etc")]
        public bool CanBeInHurt;
        public bool IsBackStep;

        [HideInInspector]
        //수동으로 입력할 수 있는 경로 필요할 듯?
        public Vector2 ActionRange => ActionDataHelper.CalculateRange(this);

        [Title("For Buff")]
        [Tooltip("공속 증가에 영향을 받는지 여부")]
        public bool IsAttack;
    }

    public static class ActionDataHelper
    {
        public static Vector2 CalculateRange(ActionData data)
        {
            if (!data.IsAttack)
                return new Vector2();
            if (ActionTypeHelper.CheckType(ActionTypeMask.None, data.AttackType))
                return new Vector2();

            var maxRange = float.MinValue;
            var minRange = float.MaxValue;

            //calc toothache range
            foreach (var bulletData in data.BulletSpawnData)
            {
                var bp = bulletData.Position.z + data.MoveCurve.Evaluate(bulletData.Frame / CommonVariables.GameFrame);
                var br = bulletData.Speed * bulletData.BulletPrefab2.bulletData.Common.LifeTimeFrame / CommonVariables.GameFrame * (float)Math.Cos(bulletData.Angle * Math.PI / 180);
                maxRange = new[] { maxRange, bp, bp + br }.Max();
                minRange = new[] { minRange, bp, bp + br }.Min();
            }

            //calc hitbox range
            foreach (var hitbox in data.AttackHitboxData)
            {
                var sp = data.MoveCurve.Evaluate(hitbox.StartFrame / CommonVariables.GameFrame);
                var sr = 10; //히트박스 사이즈를 가져오기 뭐하다
                maxRange = new[] { maxRange, sp - sr, sp + sr }.Max();
                minRange = new[] { minRange, sp - sr, sp + sr }.Min();
            }

            return new Vector2(minRange, maxRange);
        }
    }
}