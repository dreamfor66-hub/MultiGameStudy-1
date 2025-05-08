using System;
using UnityEngine;

namespace Rogue.Ingame.Dungeon
{
    public class DungeonEventModel
    {
        public Action<Vector2Int> OnStartSelect;
        public Action<Vector2Int> OnSelected;
        public Action OnNext;

        public void StartSelect(Vector2Int curPos)
        {
            OnStartSelect?.Invoke(curPos);
        }

        public void Selected(Vector2Int nextPos)
        {
            OnSelected?.Invoke(nextPos);
        }

        public void Next()
        {
            OnNext?.Invoke();
        }
    }
}