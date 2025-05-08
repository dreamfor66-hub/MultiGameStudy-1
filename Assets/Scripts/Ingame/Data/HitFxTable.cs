using UnityEngine;

namespace Rogue.Ingame.Data
{
    [CreateAssetMenu(fileName = "new HitFxTable", menuName = "Data/HitFx Table")]
    public class HitFxTable : AssetTable<HitFxData>
    {
        private static HitFxTable instance;
        public static HitFxTable Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<HitFxTable>("HitFxTable");
                return instance;
            }
        }

        public HitFxData DefaultHitstop;

        public HitFxData GetByIdOrDefault(int id)
        {
            var data = GetById(id);
            return data == null ? DefaultHitstop : data;
        }
    }
}