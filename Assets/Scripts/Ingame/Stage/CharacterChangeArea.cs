using System;
using FMLib.UI.OnOff;
using Rogue.Ingame.Character;
using Rogue.Ingame.Event;
using Rogue.Ingame.Goods;
using Rogue.Ingame.Input;
using Rogue.Ingame.Reward;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue
{
    public class CharacterChangeArea : MonoBehaviour
    {
        [SerializeField] [Required] private OnOffBehaviour activeOnOff;
        [SerializeField] int CharacterNum;

        private bool isInteractable = false;

        private void Update()
        {
            if (isInteractable && InputDetector.GetConfirmButton())
                Interact();
        }

        private void Interact()
        {
            //SetActive(false);
            SetInteractable(false);
            EventDispatcher.Send(new EventChangeCharacter(OwnerCharacterHolder.OwnerCharacter.EntityId, CharacterNum));
        }
        private void SetActive(bool active)
        {
            activeOnOff.Set(active);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == OwnerCharacterHolder.OwnerCharacter.gameObject)
                SetInteractable(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == OwnerCharacterHolder.OwnerCharacter.gameObject)
                SetInteractable(true);
        }
        private void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
        }
    }
}
