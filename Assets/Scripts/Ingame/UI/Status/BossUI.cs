using Rogue.Ingame.Character;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Status
{
    public class BossUI : MonoBehaviour
    {
        public static BossUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            Hide();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        [SerializeField] [Required] private GameObject panel;
        [SerializeField] [Required] private HpBarView hpBar;
        [SerializeField] [Required] private BuffTableView buffs;
        [SerializeField] [Required] private Text nameText;

        private CharacterBehaviour character;

        public void Bind(CharacterBehaviour chara, string name)
        {
            if (this.character != null)
            {
                Debug.LogError("duplicate bind error");
                return;
            }
            this.character = chara;
            hpBar.Bind(chara.HpModule);
            buffs.Bind(chara.BuffSync);
            nameText.text = name;
            Show();
        }

        public void Release(CharacterBehaviour chara)
        {
            if (this.character != chara)
                return;
            this.character = null;
            hpBar.Release();
            buffs.Release();
            Hide();
        }

        private void Show()
        {
            panel.SetActive(true);
        }

        private void Hide()
        {
            panel.SetActive(false);
        }

    }
}