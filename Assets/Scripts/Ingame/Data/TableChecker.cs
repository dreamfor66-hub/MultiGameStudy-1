using Rogue.Ingame.Bullet;
using Rogue.Ingame.Data.Buff;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    public static class TableChecker
    {
        public static bool IsInTable(HitFxData hitFx)
        {
            return hitFx == null || HitFxTable.Instance.Assets.Contains(hitFx);
        }

        public static bool IsInTable(BulletBehaviour bullet)
        {
            return bullet == null || BulletTable.Instance.Assets.Contains(bullet);
        }

        public static bool IsInTable(BuffData buff)
        {
            return buff == null || BuffTable.Instance.Assets.Contains(buff);
        }
    }
}