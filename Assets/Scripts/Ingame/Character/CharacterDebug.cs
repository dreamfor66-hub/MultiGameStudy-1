using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    [RequireComponent(typeof(CharacterBehaviour))]
    public class CharacterDebug : MonoBehaviour
    {
        private CharacterBehaviour character;
        private void Awake()
        {
            character = GetComponent<CharacterBehaviour>();
        }

        [Button]
        public void AddBuffServer(BuffData buff)
        {
            character.BuffAccepter.AddBuff(buff, character);
        }

        [Button]
        public void ClearBuff()
        {
            character.BuffAccepter.Clear();
        }
    }
}