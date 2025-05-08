using System.Collections.Generic;
using Rogue.Ingame.Util.Pool;
using UnityEngine;

namespace Rogue.Ingame.Vfx
{
    public static class VfxSpawner
    {
        private static readonly List<ObjectPool<VfxObject>> pools = new List<ObjectPool<VfxObject>>();

        private static ObjectPool<VfxObject> GetPool(VfxObject prefab)
        {
            var pool = pools.Find(x => x.Prefab == prefab);
            if (pool == null)
            {
                pool = new ObjectPool<VfxObject>(prefab, true);
                pools.Add(pool);
            }
            else if (!pool.IsValid)
            {
                pools.Remove(pool);
                pool = new ObjectPool<VfxObject>(prefab, true);
                pools.Add(pool);
            }
            return pool;
        }

        public static void Spawn(VfxObject prefab, Vector3 position, float scale)
        {
            var pool = GetPool(prefab);
            var obj = pool.Spawn();
            obj.transform.position = position;
            obj.transform.localScale = Vector3.one * scale;
        }

        public static VfxObject Spawn(VfxObject prefab, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            var pool = GetPool(prefab);
            var obj = pool.Spawn();
            obj.SetFollow(parent, position, rotation, scale);
            return obj;
        }
    }
}