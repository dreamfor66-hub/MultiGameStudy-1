using System.Collections.Generic;
using Rogue.Ingame.Util.Pool;

namespace Rogue.Ingame.Bullet
{
    public class BulletSpawner
    {
        private readonly List<ObjectPool<BulletBehaviour>> pools = new List<ObjectPool<BulletBehaviour>>();

        private ObjectPool<BulletBehaviour> GetPool(BulletBehaviour prefab)
        {
            var pool = pools.Find(x => x.Prefab == prefab);
            if (pool == null)
            {
                pool = new ObjectPool<BulletBehaviour>(prefab, true);
                pools.Add(pool);
            }
            else if (!pool.IsValid)
            {
                pools.Remove(pool);
                pool = new ObjectPool<BulletBehaviour>(prefab, true);
                pools.Add(pool);
            }

            return pool;
        }

        public BulletBehaviour Spawn(BulletBehaviour prefab)
        {
            var pool = GetPool(prefab);
            var obj = pool.Spawn();
            return obj;
        }
    }
}