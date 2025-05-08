using Rogue.Ingame.Character;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI.Status
{
    public class MyStatusPanel : MonoBehaviour
    {
        [SerializeField] [Required] private HpBarView hpBar;
        [SerializeField] [Required] private StaminaBarView staminaBar;
        [SerializeField] [Required] private StackView stack;

        public void Start()
        {
            hpBar.gameObject.SetActive(false);

            OwnerCharacterHolder.OnChanged += OnOwnerCharacterChanged;
            OnOwnerCharacterChanged();
        }

        private void OnOwnerCharacterChanged()
        {
            if (OwnerCharacterHolder.OwnerCharacter != null)
            {
                hpBar.Bind(OwnerCharacterHolder.OwnerCharacter.HpModule);
                hpBar.gameObject.SetActive(true);
                staminaBar.Bind(OwnerCharacterHolder.OwnerCharacter.StaminaModule);
                staminaBar.gameObject.SetActive(true);
                stack.Bind(OwnerCharacterHolder.OwnerCharacter.StackModule);
                stack.gameObject.SetActive(true);
            }
            else
            {
                hpBar.Release();
                hpBar.gameObject.SetActive(false);
                staminaBar.Release();
                staminaBar.gameObject.SetActive(false);
                stack.Release();
                stack.gameObject.SetActive(false);
            }
        }
    }
}