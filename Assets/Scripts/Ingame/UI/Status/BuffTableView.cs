using System.Collections.Generic;
using Rogue.Ingame.Buff;
using UnityEngine;

namespace Rogue.Ingame.UI.Status
{
    public class BuffTableView : MonoBehaviour
    {
        [SerializeField] private BuffIconView prefab;

        private BuffSync buffSync;
        private readonly Dictionary<int, BuffIconView> buffIcons = new Dictionary<int, BuffIconView>();
        private readonly Queue<BuffIconView> pool = new Queue<BuffIconView>();

        public void Bind(BuffSync buff)
        {
            buffSync = buff;
            Init();
        }

        private void Init()
        {
            buffSync.OnStartBuff += OnStartBuff;
            buffSync.OnChangeBuff += OnChangeBuff;
            buffSync.OnEndBuff += OnEndBuff;
        }

        public void Release()
        {
            buffSync.OnStartBuff -= OnStartBuff;
            buffSync.OnChangeBuff -= OnChangeBuff;
            buffSync.OnEndBuff -= OnEndBuff;

            buffSync = null;

            foreach (var pair in buffIcons)
                ReturnToPool(pair.Value);
            buffIcons.Clear();
        }

        private void OnStartBuff(BuffInfo buff)
        {
            if (!buff.BuffData.IconData.IsVisible)
                return;

            var obj = GetFromPool();
            obj.UpdateInfo(buff);
            buffIcons.Add(buff.BuffInstanceId, obj);
        }

        private void OnChangeBuff(BuffInfo buff)
        {
            if (!buff.BuffData.IconData.IsVisible)
                return;

            if (buffIcons.TryGetValue(buff.BuffInstanceId, out var obj))
            {
            }
            else
            {
                obj = GetFromPool();
                buffIcons.Add(buff.BuffInstanceId, obj);
            }
            obj.UpdateInfo(buff);
        }

        private void OnEndBuff(BuffEndInfo buff)
        {
            if (buffIcons.ContainsKey(buff.BuffInstanceId))
            {
                var obj = buffIcons[buff.BuffInstanceId];
                ReturnToPool(obj);
                buffIcons.Remove(buff.BuffInstanceId);
            }
        }

        private void LateUpdate()
        {
            foreach (var pair in buffIcons)
            {
                pair.Value.OnLateUpdate();
            }
        }

        private BuffIconView GetFromPool()
        {
            var obj = pool.Count != 0 ? pool.Dequeue() : Instantiate(prefab, gameObject.transform);
            obj.gameObject.SetActive(true);
            return obj;
        }

        private void ReturnToPool(BuffIconView obj)
        {
            obj.clear();
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}
