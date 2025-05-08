using FMLib.Structs;
using Photon.Bolt;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Input;

namespace Rogue.BoltAdapter
{
    public static class TokenConverter
    {
        public static CharacterTotalInfo FromToken(CharacterStateToken token, CharacterData characterData)
        {
            CharacterTotalInfo info;
            info.StateInfo.Id = token.Id;
            info.StateInfo.Frame = token.Frame;
            info.StateInfo.StateType = token.StateType;
            info.StateInfo.Direction = DirectionEncoder.FromAngle(token.Direction);
            info.StateInfo.ActionData = token.ActionData < 0
                ? null
                : characterData.PossibleActions[token.ActionData];
            info.StateInfo.KnockbackDistance = token.KnockbackDistance;
            info.StateInfo.KnockbackStrength = token.KnockbackStrength;
            info.StateInfo.KnockStopFrame = token.KnockStopFrame;
            info.StateInfo.HitstopId = token.HitstopId;
            info.StateInfo.HitstopFrame = token.HitstopFrame;
            info.StateInfo.HitstopReductionIdx = token.HitstopReductionIdx;
            info.StateInfo.StunFrame = token.StunFrame;

            info.PreInputInfo.Command = token.PreInputCommand;
            info.PreInputInfo.RemainFrame = token.PreInputFrame;
            info.PreInputInfo.Direction = token.PreInputDirection;

            info.StaminaInfo.CurStamina = token.CurStamina;
            info.StaminaInfo.NextFrame = token.StaminaNextFrame;
            info.StackInfo.CurStack = token.CurStack;

            info.Position = token.Position;
            return info;
        }

        public static CharacterStateToken ToToken(CharacterTotalInfo info, CharacterData characterData, int serverFrame)
        {
            var token = ProtocolTokenUtils.GetToken<CharacterStateToken>();
            token.Id = info.StateInfo.Id;
            token.Frame = info.StateInfo.Frame;
            token.StateType = info.StateInfo.StateType;
            token.Direction = DirectionEncoder.ToAngle(info.StateInfo.Direction);
            token.ActionData = characterData.PossibleActions.IndexOf(info.StateInfo.ActionData);
            token.Position = new VectorXZ(info.Position.x, info.Position.z);

            token.KnockbackDistance = info.StateInfo.KnockbackDistance;
            token.KnockbackStrength = info.StateInfo.KnockbackStrength;
            token.KnockStopFrame = info.StateInfo.KnockStopFrame;

            token.HitstopId = info.StateInfo.HitstopId;
            token.HitstopFrame = info.StateInfo.HitstopFrame;
            token.HitstopReductionIdx = info.StateInfo.HitstopReductionIdx;
            token.StunFrame = info.StateInfo.StunFrame;

            token.PreInputCommand = info.PreInputInfo.Command;
            token.PreInputFrame = info.PreInputInfo.RemainFrame;
            token.PreInputDirection = info.PreInputInfo.Direction;

            token.CurStamina = info.StaminaInfo.CurStamina;
            token.StaminaNextFrame = info.StaminaInfo.NextFrame;
            token.CurStack = info.StackInfo.CurStack;

            token.ServerFrame = serverFrame;
            return token;
        }

        public static HpInfo FromToken(HpToken token)
        {
            HpInfo info;
            info.CurHp = token.CurHp;
            info.MaxHp = token.MaxHp;
            info.Shield = token.Shield;
            return info;
        }

        public static HpToken ToToken(HpInfo hpInfo)
        {
            var token = ProtocolTokenUtils.GetToken<HpToken>();
            token.CurHp = hpInfo.CurHp;
            token.MaxHp = hpInfo.MaxHp;
            token.Shield = hpInfo.Shield;
            return token;
        }
    }
}