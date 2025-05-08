using Rogue.Ingame.Buff;
using Rogue.Ingame.Character;
using UnityEngine;

namespace Rogue.Ingame.UI
{
    public class CharacterUI : MonoBehaviour
    {
        [SerializeField] private HpUI hpUI;

        private CharacterBehaviour character;


        public void Show(CharacterBehaviour character)
        {
            if (this.character == character)
                return;

            this.character = character;
            if (hpUI != null)
            {
                //hpUI.SetModule(character.GetComponent<HpModule>());
            }

            gameObject.SetActive(true);
        }

        public void Update()
        {
            if (character == null)
                Hide();
        }

        public void Hide()
        {
            character = null;
            hpUI.SetModule(null);
            gameObject.SetActive(false);
        }
    }
}