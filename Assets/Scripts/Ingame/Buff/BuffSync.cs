using System;

namespace Rogue.Ingame.Buff
{
    public class BuffSync
    {
        public Action<BuffInfo> OnStartBuff;
        public Action<BuffInfo> OnChangeBuff;
        public Action<BuffEndInfo> OnEndBuff;
    }
}