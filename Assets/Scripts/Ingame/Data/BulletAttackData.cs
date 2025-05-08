using System;
using System.Collections.Generic;
using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;

namespace Rogue.Ingame.Data
{
    public enum BulletAttackConditionType
    {
        Always,
        Frame,
        Event,
    }

    public enum BulletAttackTeamFilter
    {
        Attackable,
        SameTeam,
        All,
    }

    [Serializable]
    [HideLabel]
    [Title("Condition")]
    public class BulletAttackConditionData
    {
        public BulletAttackConditionType Type;
        [ShowIf(nameof(Type), BulletAttackConditionType.Frame)]
        public int MinFrame;
        [ShowIf(nameof(Type), BulletAttackConditionType.Frame)]
        public int MaxFrame;
        [ShowIf(nameof(Type), BulletAttackConditionType.Event)]
        public BulletEvent Event;
    }

    [Serializable]
    [Toggle("Enabled")]
    public class BulletAttackData
    {
        public bool Enabled;
        public List<BulletSingleAttackData> Attacks;
        public int HitstopFrame;
    }

    [Serializable]
    public class BulletSingleAttackData
    {
        public int AnchorId;
        public int GroupId;
        public BulletAttackTeamFilter TeamFilter;
        public BulletAttackConditionData Condition;
        [HideLabel] public HitboxInfo HitboxInfo;

        [Title("Buff")]
        [ValidateInput("@TableChecker.IsInTable(BuffData)", "Buff Table 에 포함되지 않은 에셋입니다. 테이블을 업데이트 해주세요.")]
        public BuffData BuffData;
        public bool BuffOnly;

        [Title("Etc")]
        public bool HitOnce;
        public bool IsDirectHit;
    }
}