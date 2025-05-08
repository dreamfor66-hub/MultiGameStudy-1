using System.Collections.Generic;
using UnityEngine;

namespace Rogue.Ingame.Util.Pool
{
    public class UIPool<T> where T : Component
    {
        public T Prefab { get; }
        public Transform Parent { get; }

        private readonly Queue<T> pool = new Queue<T>();
        private readonly List<T> spawned = new List<T>();

        public UIPool(T prefab)
        {
            Prefab = prefab;
            Parent = prefab.transform.parent;

            prefab.gameObject.SetActive(false);
        }

        public T Get()
        {
            var obj = pool.Count > 0 ? pool.Dequeue() : Create();
            obj.gameObject.SetActive(true);
            spawned.Add(obj);
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            spawned.Remove(obj);
            pool.Enqueue(obj);
        }

        public void Clear()
        {
            foreach (var obj in spawned)
            {
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }

            spawned.Clear();
        }

        private T Create()
        {
            var obj = Object.Instantiate(Prefab, Parent);
            return obj;
        }
    }
}