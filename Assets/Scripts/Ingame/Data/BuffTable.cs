using Rogue.Ingame.Data.Buff;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    [CreateAssetMenu(fileName = "new BuffTable", menuName = "Data/Buff Table")]
    public class BuffTable : AssetTable<BuffData>
    {
        private static BuffTable instance;

        public static BuffTable Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<BuffTable>("BuffTable");
                return instance;
            }
        }
    }
}