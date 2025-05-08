using Rogue.Ingame.Bullet;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    [CreateAssetMenu(fileName = "new BulletTable", menuName = "Data/Bullet Table")]
    public class BulletTable : AssetTable<BulletBehaviour>
    {
        private static BulletTable instance;

        public static BulletTable Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<BulletTable>("BulletTable");
                return instance;
            }
        }
    }
}