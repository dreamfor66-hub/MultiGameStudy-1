using Rogue.Ingame.Character;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Status
{
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] [Required] private GameObject panel;
        [SerializeField] [Required] private Image slideImage;

        private CharacterInteraction characterInteraction = null;
        public void Start()
        {
            OwnerCharacterHolder.OnChanged += OnOwnerCharacterChanged;
            OnOwnerCharacterChanged();
        }

        private void OnOwnerCharacterChanged()
        {
            if (OwnerCharacterHolder.OwnerCharacter != null)
            {
                characterInteraction = OwnerCharacterHolder.OwnerCharacter.GetComponent<CharacterInteraction>();
            }
            else
            {
                characterInteraction = null;
            }
        }

        private void Update()
        {
            if (characterInteraction == null)
            {
                panel.SetActive(false);
                return;
            }

            panel.SetActive(characterInteraction.NowInteract);
            if (characterInteraction.NowInteract)
            {
                slideImage.fillAmount = characterInteraction.InteractRatio;
            }
        }
    }
}
