using Rogue.Ingame.Event;
using Rogue.Ingame.Util.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI.Status
{
    public class CoinUIPool : MonoBehaviour
    {
        [SerializeField] [Required] private CoinGetUI prefab;
        private UIPool<CoinGetUI> pool;

        private void Awake()
        {
            pool = new UIPool<CoinGetUI>(prefab);
            EventDispatcher.Listen<EventCoinGet>(OnEventCoinGet);
        }

        private void OnDestroy()
        {
            EventDispatcher.Remove<EventCoinGet>(OnEventCoinGet);
        }

        private void OnEventCoinGet(EventCoinGet evt)
        {
            var coinUI = pool.Get();
            coinUI.Show(evt.Amount, evt.Entity.GameObject.transform, new Vector3(0f, 3.7f, 0f));
            coinUI.RegisterDespawn(() => { pool.Return(coinUI); });
        }

    }
}