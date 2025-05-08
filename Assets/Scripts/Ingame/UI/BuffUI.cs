using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI
{
    [System.Serializable]
    public class BuffUIData
    {
        public BuffTag Tag;
        [PreviewField] public Sprite Sprite;
        public bool ShowStack;
    }

    public class BuffUI : MonoBehaviour
    {
        [SerializeField][Required] private CharacterBehaviour character;
        [SerializeField][Required] private BuffIconUI prefab;
        [SerializeField][TableList] private List<BuffUIData> uiData;

        private readonly Stack<BuffIconUI> pool = new Stack<BuffIconUI>();
        private List<BuffIconUI> showing = new List<BuffIconUI>();

        private void Start()
        {
            var buffAccepter = character.BuffAccepter;
            foreach (var curBuff in buffAccepter.GetBuffs())
                OnStartBuff(curBuff);
        }

        private void OnDestroy()
        {
            var buffAccepter = character.BuffAccepter;
        }

        private void OnStartBuff(BuffInstance buff)
        {
            var find = uiData.Find(x => buff.Data.Tags.Any(t => t == x.Tag));
            if (find == null)
                return;

            var icon = GetFromPool();
            icon.Set(find.Sprite, buff, find.ShowStack);
        }

        private void OnEndBuff(BuffInstance buff)
        {
            var find = showing.Find(x => x.Buff == buff);
            if (find == null)
                return;

            find.Clear();
            ReturnToPool(find);
        }

        private BuffIconUI GetFromPool()
        {
            var obj = pool.Count != 0 ? pool.Peek() : Instantiate(prefab, prefab.transform.parent);
            obj.gameObject.SetActive(true);
            showing.Add(obj);
            return obj;
        }

        private void ReturnToPool(BuffIconUI obj)
        {
            obj.gameObject.SetActive(false);
            showing.Remove(obj);
            pool.Push(obj);
        }

        public void SetCharacter(CharacterBehaviour character)
        {
            this.character = character;
        }
    }
}