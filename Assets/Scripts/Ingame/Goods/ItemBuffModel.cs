using System;
using Rogue.Ingame.Data.Buff;

namespace Rogue.Ingame.Goods
{
    public class ItemBuffModel
    {
        public Action<BuffData> OnAddBuff;

        public void AddBuff(BuffData buff)
        {
            OnAddBuff?.Invoke(buff);
        }
    }
}