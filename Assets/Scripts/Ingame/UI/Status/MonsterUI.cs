using Rogue.Ingame.Character;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI.Status
{
    public class MonsterUI : MonoBehaviour
    {
        [SerializeField] [Required] private RectTransform rect;
        [SerializeField] [Required] private HpBarView hpBar;
        [SerializeField] [Required] private BuffTableView buffs;
        private bool isShow = false;
        private CharacterBehaviour character;

        public void Init(CharacterBehaviour character)
        {
            this.character = character;
            hpBar.Bind(character.HpModule);
            buffs.Bind(character.BuffSync);
            Hide();
        }

        public void Release()
        {
            character = null;
            hpBar.Release();
            buffs.Release();
        }

        private void Hide()
        {
            isShow = false;
            hpBar.gameObject.SetActive(false);
        }

        private void Show()
        {
            isShow = true;
            hpBar.gameObject.SetActive(true);
        }

        public void Update()
        {
            if (character == null)
                return;
            if (!isShow)
            {
                var info = character.HpModule.HpInfo;
                if (info.CurHp < info.MaxHp)
                    Show();
            }

            if (isShow)
            {
                if (hpBar.IsDead)
                    Hide();
            }
        }

        public void SetPosition(Vector2 viewPort)
        {
            rect.anchorMin = viewPort;
            rect.anchorMax = viewPort;
            rect.anchoredPosition = Vector2.zero;
        }
    }
}
