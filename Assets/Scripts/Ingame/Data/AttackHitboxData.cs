using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Vfx;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    [System.Serializable]
    public class HitboxInfo
    {
        [Title("Knockback")]
        [HideLabel]
        public KnockbackData Knockback;

        [Title("HitFx")]
        [ValidateInput("@TableChecker.IsInTable(HitFx)", "HitFx Table 에 포함되지 않은 에셋입니다. 테이블을 업데이트 해주세요.")]
        public HitFxData HitFx;

        [Title("Damage")]
        public int Damage;
    }


    [System.Serializable]
    public class AttackHitboxData
    {
        public int GroupId;
        public int ColliderId;
        public int StartFrame;
        public int EndFrame;

        [HideLabel]
        public HitboxInfo Info;

        [Title("Buff")]
        [ValidateInput("@TableChecker.IsInTable(BuffData)", "Buff Table 에 포함되지 않은 에셋입니다. 테이블을 업데이트 해주세요.")]
        public BuffData BuffData;
    }

    [System.Serializable]
    public class ActionVfxData
    {
        public VfxObject Prefab;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public int StartFrame;
    }
}