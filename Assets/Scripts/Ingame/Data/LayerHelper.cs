using UnityEngine;

namespace Rogue.Ingame.Data
{
    public static class LayerHelper
    {
        public static int HurtboxMask => LayerMask.GetMask("Hurtbox");
        public static int GhostLayer => LayerMask.NameToLayer("Character_Ghost");
        public static int MapMask => LayerMask.GetMask("Ground");

        public static int HurtboxLayer => LayerMask.NameToLayer("Hurtbox");
        public static int CharacterPlayerLayer => LayerMask.NameToLayer("Character_Player");
        public static int CharacterMonsterLayer => LayerMask.NameToLayer("Character_Monster");
        public static int InteractableLayer => LayerMask.NameToLayer("Interactable");

    }
}