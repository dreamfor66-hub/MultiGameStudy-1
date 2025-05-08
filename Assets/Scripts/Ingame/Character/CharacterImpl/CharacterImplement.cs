using UnityEngine;

namespace Rogue.Ingame.Character.CharacterImpl
{
    public abstract class CharacterImplement : MonoBehaviour
    {
        public abstract void InitNetworkState(bool logic, bool visual, bool controller);
        public abstract void UpdateStateInfo(CharacterStateUpdateInfo updateInfo);
        public abstract void UpdateMove(CharacterStateUpdateInfo updateInfo);
    }
}