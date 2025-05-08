using System;

namespace Rogue.Ingame.Character
{
    public static class OwnerCharacterHolder
    {
        public static bool OwnerCharacterExistAndAlive => OwnerCharacter != null && !OwnerCharacter.IsDead;
        public static CharacterBehaviour OwnerCharacter { get; private set; }
        public static Action OnChanged;

        public static void SetPlayer(CharacterBehaviour character)
        {
            OwnerCharacter = character;
            OnChanged?.Invoke();
        }
    }
}
