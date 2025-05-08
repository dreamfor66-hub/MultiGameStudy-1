using Rogue.Ingame.Data;

namespace Rogue.Ingame.Character
{
    public struct CharacterStateUpdateInfo
    {
        public CharacterStateInfo Prev;
        public CharacterStateInfo Cur;
        public float DeltaFrame => IsStateChanged ? Cur.Frame : Cur.Frame - Prev.Frame;

        public bool IsStateChanged => Prev.Id != Cur.Id || Prev.StateType != Cur.StateType || Prev.ActionData != Cur.ActionData;
        public float DeltaTime => DeltaFrame / CommonVariables.GameFrame;

        public CharacterStateUpdateInfo(CharacterStateInfo prev, CharacterStateInfo cur)
        {
            Prev = prev;
            Cur = cur;
        }
    }
}