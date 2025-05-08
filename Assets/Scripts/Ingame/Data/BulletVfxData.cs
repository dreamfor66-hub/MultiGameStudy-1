using System;
using System.Collections.Generic;
using Rogue.Ingame.Vfx;
using Sirenix.OdinInspector;

namespace Rogue.Ingame.Data
{
    [Serializable]
    public class BulletEventVfxData
    {
        public BulletEvent Trigger;
        public VfxObject Prefab;
        public float Scale = 1f;
    }

    [Serializable]
    [Toggle("Enabled")]
    public class BulletVfxData
    {
        public bool Enabled;
        public List<BulletEventVfxData> Vfxs;
    }

    [Serializable]
    [Toggle("Enabled")]
    public class BulletMoveParabolaData
    {
        public bool Enabled;
        public float GravityAccel;
        public float Resistance;
    }
}