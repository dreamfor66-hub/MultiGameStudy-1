using FMLib.UI.OnOff;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    [RequireComponent(typeof(Collider))]
    public abstract class InteractableObject : MonoBehaviour
    {
        [SerializeField] [Required] private new Collider collider;
        [SerializeField] [Required] private OnOffBehaviour onOffInteractable;

        public bool IsSelected { get; private set; }
        public bool IsEnabled { get; private set; }
        public virtual int InteractionFrame => 60;

        public virtual void SetSelected(bool select)
        {
            IsSelected = select;
            onOffInteractable.Set(select);
        }

        public virtual void SetEnable(bool enable)
        {
            IsEnabled = enable;
            collider.enabled = enable;
            if (!enable && IsSelected)
                SetSelected(false);
        }

        public abstract void Interact(CharacterBehaviour character);
    }
}