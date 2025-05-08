using System;
using Sirenix.OdinInspector;

namespace Rogue.Ingame.Data
{
    // 타격 회수로 감소하는 수치를 Hp 와 구분하여 Dp(Durability Point) 로 네이밍 한다.
    [Toggle("Enabled")]
    [Serializable]
    public class DpData
    {
        public bool Enabled;
        public int MaxDp;

        public bool isUseDp()
        {
            return Enabled;
        }
    }
}