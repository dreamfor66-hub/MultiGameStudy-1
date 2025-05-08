using Rogue.Ingame.Event;
using Rogue.Ingame.Util.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI.Status
{
    public class DamageUIPool : MonoBehaviour
    {
        [SerializeField] [Required] private DamageUI prefab;
        private UIPool<DamageUI> pool;

        private void Awake()
        {
            pool = new UIPool<DamageUI>(prefab);
            EventDispatcher.Listen<EventDamage>(OnEventDamage);
            EventDispatcher.Listen<EventHeal>(OnEventHeal);
        }

        private void OnDestroy()
        {
            EventDispatcher.Remove<EventDamage>(OnEventDamage);
            EventDispatcher.Remove<EventHeal>(OnEventHeal);
        }

        private void OnEventDamage(EventDamage evt)
        {
            var damageUI = pool.Get();
            damageUI.ShowDamage(evt.Damage, evt.Critical, evt.Position);
            damageUI.RegisterDespawn(() => { pool.Return(damageUI); });
        }

        private void OnEventHeal(EventHeal evt)
        {
            var damageUI = pool.Get();
            damageUI.ShowHeal(evt.Amount, evt.Position);
            damageUI.RegisterDespawn(() => { pool.Return(damageUI); });
        }
    }
}