using Rogue.Ingame.Event;
using UnityEngine;

namespace Rogue.Ingame.UI
{
    public class BossCharacterUI : MonoBehaviour
    {
        [SerializeField] private CharacterUI characterUI;

        public void Start()
        {
            EventDispatcher.Listen<EventAttackHit>(OnEventAttackHit);
        }

        public void OnDestroy()
        {
            EventDispatcher.Remove<EventAttackHit>(OnEventAttackHit);
        }

        private void OnEventAttackHit(EventAttackHit evt)
        {
            // if (evt.Hurtable.IsBoss)
            // {
            //     var boss = evt.Hurtable.GetComponent<CharacterMain>();
            //     characterUI.Show(boss);
            // }
        }
    }
}