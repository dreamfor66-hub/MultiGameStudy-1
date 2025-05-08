using System.Collections.Generic;
using FMLib.Extensions;
using Rogue.Ingame.Data;
using Rogue.Ingame.Input;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class CharacterInteraction : MonoBehaviour
    {
        private CharacterBehaviour Character { get; set; }
        public bool NowInteract { get; private set; }
        public int InteractMaxFrame { get; private set; }
        public int InteractElapsedFrame { get; private set; }
        public float InteractRatio => InteractMaxFrame == 0 ? 1f : (float)InteractElapsedFrame / InteractMaxFrame;
        private InteractableObject interactionObject;

        void Awake()
        {
            Character = GetComponent<CharacterBehaviour>();
        }

        void FixedUpdate()
        {
            InteractableObject curSelected = null;
            list.RemoveAll(x => x == null || !x.IsEnabled);
            if (list.Count > 0)
                curSelected = list.MinBy(x => Vector3.Distance(transform.position, x.transform.position));
            Select(curSelected);

            if (curSelected != null && InputDetector.GetConfirmButton() && IsInteractableState() && !NowInteract)
            {
                StartInteract(curSelected);
            }

            if (NowInteract)
            {
                if (!IsInteractableState() || interactionObject == null || !interactionObject.IsEnabled)
                    StopInteract();
                else
                {
                    InteractElapsedFrame++;
                    if (InteractElapsedFrame >= InteractMaxFrame)
                    {
                        CompleteInteract();
                    }
                }
            }

        }

        private void StartInteract(InteractableObject obj)
        {
            NowInteract = true;
            InteractMaxFrame = obj.InteractionFrame;
            InteractElapsedFrame = 0;
            interactionObject = obj;
        }


        private void StopInteract()
        {
            NowInteract = false;
            InteractMaxFrame = 0;
            InteractElapsedFrame = 0;
            interactionObject = null;
        }

        private void CompleteInteract()
        {
            interactionObject.Interact(Character);
            StopInteract();
        }

        private bool IsInteractableState()
        {
            return Character.Character.StateInfo.StateType == CharacterStateType.Idle;
        }

        private void Select(InteractableObject obj)
        {
            if (selected == obj)
                return;
            if (selected != null)
                selected.SetSelected(false);
            if (obj != null)
                obj.SetSelected(true);
            selected = obj;
        }

        private List<InteractableObject> list = new List<InteractableObject>();
        private InteractableObject selected = null;


        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.parent == this.transform)
                return;
            if (other.gameObject.layer == LayerHelper.InteractableLayer)
            {
                var interactable = other.gameObject.GetComponent<InteractableObject>();
                if (interactable != null)
                    list.Add(interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.parent == this.transform)
                return;
            if (other.gameObject.layer == LayerHelper.InteractableLayer)
            {
                var interactable = other.gameObject.GetComponent<InteractableObject>();
                if (interactable != null)
                    list.Remove(interactable);
            }
        }
    }
}