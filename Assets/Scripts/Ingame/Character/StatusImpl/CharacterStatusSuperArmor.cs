using Rogue.Ingame.Attack;
using UnityEngine;

namespace Rogue.Ingame.Character.StatusImpl
{
    public class CharacterStatusSuperArmor : ICharacterStatusImpl
    {
        private Hurtbox[] hurtboxes;
        public CharacterStatusSuperArmor(GameObject obj)
        {
            hurtboxes = obj.GetComponentsInChildren<Hurtbox>();
        }
        public void TurnOn()
        {
            foreach (var hurtbox in hurtboxes)
                hurtbox.SetStatusSuperArmor(true);
        }

        public void TurnOff()
        {
            foreach (var hurtbox in hurtboxes)
                hurtbox.SetStatusSuperArmor(false);
        }
    }
}