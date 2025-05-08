using System.Collections.Generic;
using UnityEngine;

namespace Rogue.Ingame.Util.Pool
{
    public class ObjectPool<T> where T : MonoBehaviour, ISpawnable
    {
        public T Prefab { get; }
        public Transform Parent { get; }

        private readonly Queue<T> pool = new Queue<T>();

        public ObjectPool(T prefab, bool dontDestroyOnLoad)
        {
            Prefab = prefab;
            Parent = new GameObject($"[Pool] {Prefab.name}").transform;
            if (dontDestroyOnLoad)
                Object.DontDestroyOnLoad(Parent);
        }

        public bool IsValid => Parent != null;

        public T Spawn()
        {
            var obj = pool.Count > 0 ? pool.Dequeue() : Create();
            obj.transform.SetParent(Parent);
            obj.OnSpawn();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Despawn(T obj)
        {
            obj.OnDespawn();
            if (obj.gameObject != null)
                obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }

        private T Create()
        {
            var obj = Object.Instantiate(Prefab);
            obj.RegisterDespawn(() => { Despawn(obj); });
            return obj;
        }
    }
}