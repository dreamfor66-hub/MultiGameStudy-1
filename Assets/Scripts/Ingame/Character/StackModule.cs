using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public struct StackInfo
    {
        public int CurStack;
    }

    public class StackModule : IActionResource
    {
        public StackInfo Info => new StackInfo
        {
            CurStack = curStack,
        };

        public int MaxStack
        {
            get;
            private set;
        }
        private int curStack;

        public StackModule(CharacterStackData data)
        {
            MaxStack = data.BasicMaxStack;
            curStack = data.InitStack;
        }

        public void Sync(StackInfo stackInfo)
        {
            curStack = stackInfo.CurStack;
        }

        public bool CanUse(int count)
        {
            return count <= curStack;
        }

        public void Use(int count)
        {
            curStack -= count;
        }

        public void Add(int count)
        {
            curStack = Mathf.Clamp(curStack + count, 0, MaxStack);
        }
    }
}