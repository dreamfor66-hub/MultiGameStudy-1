using System;
using System.Collections.Generic;
using Rogue.Ingame.Vfx;
using Sirenix.OdinInspector;

namespace Rogue.Ingame.Data.Buff
{
    public enum BuffEvent
    {
        Start,
        End,
    }

    [Serializable]
    public class BuffEventVfxData
    {
        public BuffEvent Trigger;
        //public int EveryValueFrame;
        public VfxObject Prefab;
        public float Scale = 1f;
    }

    [Serializable]
    [Toggle("Enabled")]
    public class BuffVfxData
    {
        public bool Enabled;
        public List<BuffEventVfxData> Vfxs;
    }
}