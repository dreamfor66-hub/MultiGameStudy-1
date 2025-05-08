using Rogue.Ingame.Data;
using Rogue.Ingame.Vfx;
using UnityEngine;

namespace Rogue.Ingame.Bullet
{
    public class BulletVfx
    {
        private readonly Transform tm;
        private readonly BulletVfxData vfxData;

        public BulletVfx(Transform tm, BulletVfxData vfxData)
        {
            this.tm = tm;
            this.vfxData = vfxData;
        }

        public void OnEvent(Data.BulletEvent evt)
        {
            if (!vfxData.Enabled)
                return;

            foreach (var vfx in vfxData.Vfxs)
            {
                if (vfx.Trigger == evt)
                    VfxSpawner.Spawn(vfx.Prefab, tm.position, vfx.Scale);
            }
        }
    }
}