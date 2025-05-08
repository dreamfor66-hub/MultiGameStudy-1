using System.Collections.Generic;
using FMLib.Structs;
using Rogue.Ingame.Data;

namespace Rogue.Ingame.Character
{
    public struct CharacterTotalInfo
    {
        public CharacterStateInfo StateInfo;
        public StaminaInfo StaminaInfo;
        public StackInfo StackInfo;
        public PreInputInfo PreInputInfo;
        public VectorXZ Position;
    }

    public struct CharacterStateInfo
    {
        public int Id;
        public CharacterStateType StateType;
        public float Frame;
        public ActionData ActionData;
        public VectorXZ Direction;
        public float KnockbackDistance;
        public KnockbackStrength KnockbackStrength;
        public int KnockStopFrame;
        public int HitstopId;
        public int HitstopFrame;
        public int HitstopReductionIdx;
        public int StunFrame;
    }

    public struct PreInputInfo
    {
        public CharacterCommandType Command;
        public int RemainFrame;
        public VectorXZ Direction;
    }

    public struct CharacterControlInfo
    {
        public CharacterCommandType MainCommand;
        public CharacterCommandType BasicState;
        public CharacterCommandType SpecialState;
        //public CharacterCommandType SkillAState;
        //public CharacterCommandType SkillSState;
        //public CharacterCommandType SkillDState;
        public VectorXZ Direction;

        public IEnumerable<CharacterCommandType> StateCommands
        {
            get
            {
                yield return BasicState;
                yield return SpecialState;
                //yield return SkillAState;
                //yield return SkillSState;
                //yield return SkillDState;
            }
        }
    }

    public struct CharacterControlActionInfo
    {
        public ActionData Action;
        public VectorXZ Direction;
        public bool Walk;
    }

    public struct CharacterCommandDirection
    {
        public CharacterCommandType Command;
        public VectorXZ Direction;
    }
}