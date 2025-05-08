using Rogue.Ingame.Attack;
using UnityEngine;

namespace Rogue.Ingame.Character.StatusImpl
{
    public class CharacterStatusParrying : ICharacterStatusImpl
    {
        private Hurtbox[] hurtboxes;
        public CharacterStatusParrying(GameObject obj)
        {
            hurtboxes = obj.GetComponentsInChildren<Hurtbox>();
        }
        public void TurnOn()
        {
            foreach (var hurtbox in hurtboxes)
                hurtbox.SetParrying(true);
        }

        public void TurnOff()
        {
            foreach (var hurtbox in hurtboxes)
                hurtbox.SetParrying(false);
        }
    }
}