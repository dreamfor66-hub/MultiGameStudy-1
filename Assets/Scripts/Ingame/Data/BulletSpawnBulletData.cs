using System;
using System.Collections.Generic;
using Rogue.Ingame.Bullet;
using Sirenix.OdinInspector;

namespace Rogue.Ingame.Data
{
    public enum BulletSpawnBulletConditionType
    {
        Frame,
        Event,
    }

    [Serializable]
    public class BulletSpawnSingleData
    {
        public BulletSpawnBulletConditionType ConditionType;

        [ShowIf(nameof(ConditionType), BulletSpawnBulletConditionType.Frame)]
        public int ConditionFrame;
        [ShowIf(nameof(ConditionType), BulletSpawnBulletConditionType.Event)]
        public BulletEvent ConditionEvent;

        [ValidateInput("@TableChecker.IsInTable(Prefab)", "Bullet Table 에 포함되지 않은 에셋입니다. 테이블을 업데이트 해주세요.")]
        public BulletBehaviour Prefab;
        public float Speed;
        public float Angle;
        public float AngleY;
    }

    [Toggle("Enabled")]
    [Serializable]
    public class BulletSpawnBulletData
    {
        public bool Enabled;
        [TableList]
        public List<BulletSpawnSingleData> Bullets = new List<BulletSpawnSingleData>();
    }
}