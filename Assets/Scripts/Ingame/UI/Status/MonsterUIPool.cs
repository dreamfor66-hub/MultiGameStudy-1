using Rogue.Ingame.Util.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI.Status
{
    public class MonsterUIPool : MonoBehaviour
    {
        public static MonsterUIPool Instance { get; private set; }

        [SerializeField] [Required] private MonsterUI prefab;
        private UIPool<MonsterUI> pool;

        private void Awake()
        {
            pool = new UIPool<MonsterUI>(prefab);
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public MonsterUI Get()
        {
            return pool.Get();
        }

        public void Return(MonsterUI ui)
        {
            pool.Return(ui);
        }
    }
}
