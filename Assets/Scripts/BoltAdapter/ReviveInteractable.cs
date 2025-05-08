using FMLib.UI.OnOff;
using Photon.Bolt;
using Rogue.Ingame.Character;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    public class ReviveInteractable : InteractableObject
    {
        [SerializeField] [Required] private OnOffBehaviour OnOffDead;
        [SerializeField] [Required] private CharacterBehaviour character;
        public override int InteractionFrame => 120;

        private void Start()
        {
            SetEnable(false);
            SetSelected(false);
        }

        private void Update()
        {
            OnOffDead.Set(character.IsDead);

            if (!IsEnabled && character.IsDead)
                SetEnable(true);
            else if (IsEnabled && !character.IsDead)
                SetEnable(false);
        }

        public override void Interact(CharacterBehaviour interactionCharacter)
        {
            var evt = RequestReviveEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered);
            evt.TargetId = character.EntityId;
            evt.Send();
        }
    }
}