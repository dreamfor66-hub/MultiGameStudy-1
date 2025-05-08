using Rogue.Ingame.Input;

namespace Rogue.Ingame.Character
{
    public class InputConverter
    {
        private InputState prevState;

        public CharacterControlInfo Convert(InputState input)
        {
            CharacterControlInfo info;
            info.MainCommand = CharacterCommandType.None;
            if (input.Basic && !prevState.Basic)
                info.MainCommand = CharacterCommandType.BasicAttack;
            if (input.Special && !prevState.Special)
                info.MainCommand = CharacterCommandType.SpecialAttack;
            if (input.Evade && !prevState.Evade)
                info.MainCommand = CharacterCommandType.Evade;
            if (input.Ultimate && !prevState.Ultimate)
                info.MainCommand = CharacterCommandType.Ultimate;
            if (input.SkillA && !prevState.SkillA)
                info.MainCommand = CharacterCommandType.SkillA;
            if (input.SkillS && !prevState.SkillS)
                info.MainCommand = CharacterCommandType.SkillS;
            if (input.SkillD && !prevState.SkillD)
                info.MainCommand = CharacterCommandType.SkillD;

            info.BasicState = input.Basic ? CharacterCommandType.BasicAttackDown : CharacterCommandType.BasicAttackUp;
            info.SpecialState = input.Special ? CharacterCommandType.SpecialAttackDown : CharacterCommandType.SpecialAttackUp;
            //info.SkillAState = input.SkillA ? CharacterCommandType.SkillADown : CharacterCommandType.SkillAUp;
            //info.SkillSState = input.SkillS ? CharacterCommandType.SkillSDown : CharacterCommandType.SkillSUp;
            //info.SkillDState = input.SkillD ? CharacterCommandType.SkillDDown : CharacterCommandType.SkillDUp;

            info.Direction = DirectionEncoder.Decode(input.Direction);
            prevState = input;
            return info;
        }
    }
}