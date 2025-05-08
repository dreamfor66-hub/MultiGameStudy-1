using System.Collections.Generic;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Vfx;
using UnityEngine;

namespace Rogue.Ingame.Buff
{
    public class BuffVfxLegacy
    {
        private readonly Transform tm;
        private readonly BuffVfxData vfxData;
        private List<VfxObject> vfxPool;

        public BuffVfxLegacy(Transform tm, BuffVfxData vfxData)
        {
            this.tm = tm;
            this.vfxData = vfxData;

            vfxPool = new List<VfxObject>();
        }


        public void OnStart()
        {
            if (!vfxData.Enabled)
                return;

            foreach (var vfx in vfxData.Vfxs)
            {
                if (vfx.Trigger == BuffEvent.Start)
                    vfxPool.Add(VfxSpawner.Spawn(vfx.Prefab, tm, vfx.Prefab.transform.position, vfx.Prefab.transform.rotation, new Vector3(vfx.Scale, vfx.Scale, vfx.Scale)));
            }
        }

        public void OnEnd()
        {
            if (!vfxData.Enabled)
                return;

            foreach(var obj in vfxPool)
            {
                if (obj.VfxType != VfxType.Time)
                    obj.DespawnManually();
            }
            foreach (var vfx in vfxData.Vfxs)
            {
                if (vfx.Trigger == BuffEvent.End)
                    vfxPool.Add(VfxSpawner.Spawn(vfx.Prefab, tm, vfx.Prefab.transform.position, vfx.Prefab.transform.rotation, new Vector3(vfx.Scale, vfx.Scale, vfx.Scale)));
            }
        }
        public void OnUpdate(int elapsedFrame)
        {
            /*
            if (!vfxData.Enabled)
                return;
            
            foreach (var vfx in vfxData.Vfxs)
            {
                if(vfx.EveryValueFrame == 0)
                    continue;
                if(elapsedFrame % vfx.EveryValueFrame == 0)
                    vfxPool.Add(VfxSpawner.Spawn(vfx.Prefab, tm, vfx.Prefab.transform.position, vfx.Prefab.transform.rotation, new Vector3(vfx.Scale, vfx.Scale, vfx.Scale)));
            }
            */
        }
    }
}