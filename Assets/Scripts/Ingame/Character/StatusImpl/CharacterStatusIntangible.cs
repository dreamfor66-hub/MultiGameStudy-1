using Rogue.Ingame.Attack;
using UnityEngine;

namespace Rogue.Ingame.Character.StatusImpl
{
    public class CharacterStatusIntangible : ICharacterStatusImpl
    {
        private readonly Hurtbox[] hurtboxes;
        public CharacterStatusIntangible(GameObject gameObject)
        {
            hurtboxes = gameObject.GetComponentsInChildren<Hurtbox>();
        }
        public void TurnOn()
        {
            foreach (var hurtbox in hurtboxes)
            {
                hurtbox.gameObject.SetActive(false);
            }
        }

        public void TurnOff()
        {
            foreach (var hurtbox in hurtboxes)
            {
                hurtbox.gameObject.SetActive(true);
            }
        }
    }
}