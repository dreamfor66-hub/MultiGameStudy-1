using System.Collections.Generic;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Vfx;
using UnityEngine;

namespace Rogue.Ingame.Buff
{
    public class BuffVfx
    {
        private readonly Transform tm;
        private List<VfxObject> vfxPool;
        BuffSync buffSync;
        public BuffVfx(Transform tm, BuffSync buffSync)
        {
            this.tm = tm;
            this.buffSync = buffSync;
            vfxPool = new List<VfxObject>();

            buffSync.OnStartBuff += OnStart;
            buffSync.OnEndBuff += OnEnd;
        }

        public void OnStart(BuffInfo buffInfo)
        {
            if (!buffInfo.BuffData.VfxData.Enabled)
                return;

            foreach (var vfx in buffInfo.BuffData.VfxData.Vfxs)
            {
                if (vfx.Trigger == BuffEvent.Start)
                    vfxPool.Add(VfxSpawner.Spawn(vfx.Prefab, tm, vfx.Prefab.transform.position, vfx.Prefab.transform.rotation, new Vector3(vfx.Scale, vfx.Scale, vfx.Scale)));
            }
        }

        public void OnEnd(BuffEndInfo buffInfo)
        {
            if (!buffInfo.BuffData.VfxData.Enabled)
                return;

            foreach (var obj in vfxPool)
            {
                if (obj.VfxType != VfxType.Time)
                    obj.DespawnManually();
            }
            foreach (var vfx in buffInfo.BuffData.VfxData.Vfxs)
            {
                if (vfx.Trigger == BuffEvent.End)
                    vfxPool.Add(VfxSpawner.Spawn(vfx.Prefab, tm, vfx.Prefab.transform.position, vfx.Prefab.transform.rotation, new Vector3(vfx.Scale, vfx.Scale, vfx.Scale)));
            }
        }
    }
}