using FMLib.Structs;
using Photon.Bolt;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Input;
using UdpKit;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    public class CharacterStateToken : PooledProtocolToken
    {
        public int Id;
        public float Frame;
        public CharacterStateType StateType;
        public float Direction;
        public VectorXZ Position;
        public int ServerFrame;
        public int ActionData;

        public CharacterCommandType PreInputCommand;
        public int PreInputFrame;
        public VectorXZ PreInputDirection;

        public float KnockbackDistance;
        public KnockbackStrength KnockbackStrength;
        public int KnockStopFrame;

        public int HitstopId;
        public int HitstopFrame;
        public int HitstopReductionIdx;

        public int StunFrame;

        public int CurStamina;
        public int StaminaNextFrame;
        public int CurStack;

        public override void Read(UdpPacket packet)
        {
            Id = packet.ReadInt(16);
            Frame = packet.ReadFloat();
            StateType = (CharacterStateType)packet.ReadInt(8);
            Direction = packet.ReadFloat();
            var x = packet.ReadFloat();
            var z = packet.ReadFloat();
            Position = new VectorXZ(x, z);
            ServerFrame = packet.ReadInt();
            ActionData = packet.ReadInt(8);
            if (ActionData == 255)
                ActionData = -1;

            PreInputCommand = (CharacterCommandType)packet.ReadInt(3);
            PreInputFrame = packet.ReadInt(5);
            PreInputDirection = DirectionEncoder.Decode(packet.ReadInt(6));

            KnockbackDistance = packet.ReadFloat();
            KnockbackStrength = (KnockbackStrength)packet.ReadInt(4);
            KnockStopFrame = packet.ReadInt(8);

            HitstopId = packet.ReadInt(16);
            if (HitstopId == 65535)
                HitstopId = -1;

            var sign = packet.ReadBool();
            HitstopFrame = packet.ReadInt(8) * (sign ? 1 : -1);
            HitstopReductionIdx = packet.ReadInt(3);
            StunFrame = packet.ReadInt(10);

            CurStamina = packet.ReadInt(8);
            StaminaNextFrame = packet.ReadInt(10);
            CurStack = packet.ReadInt(8);
        }

        public override void Write(UdpPacket packet)
        {
            packet.WriteInt(Id, 16);
            packet.WriteFloat(Frame);
            packet.WriteInt((int)StateType, 8);
            packet.WriteFloat(Direction);
            packet.WriteFloat(Position.x);
            packet.WriteFloat(Position.z);
            packet.WriteInt(ServerFrame);
            packet.WriteInt(ActionData, 8);

            packet.WriteInt((int)PreInputCommand, 3);
            packet.WriteInt(PreInputFrame, 5);
            packet.WriteInt(DirectionEncoder.Encode(PreInputDirection), 6);

            packet.WriteFloat(KnockbackDistance);
            packet.WriteInt((int)KnockbackStrength, 4);
            packet.WriteInt(KnockStopFrame, 8);

            packet.WriteInt(HitstopId, 16);
            packet.WriteBool(HitstopFrame >= 0);
            packet.WriteInt(Mathf.Abs(HitstopFrame), 8);
            packet.WriteInt(HitstopReductionIdx, 3);
            packet.WriteInt(StunFrame, 10);

            packet.WriteInt(CurStamina, 8);
            packet.WriteInt(StaminaNextFrame, 10);
            packet.WriteInt(CurStack, 8);
        }

        public override void Reset()
        {
            Id = default;
            Frame = default;
            StateType = default;
            Direction = default;
            Position = default;
            ServerFrame = default;
            ActionData = default;

            PreInputCommand = default;
            PreInputFrame = default;
            PreInputDirection = default;

            KnockbackDistance = default;
            KnockbackStrength = default;
            KnockStopFrame = default;

            HitstopId = default;
            HitstopFrame = default;
            StunFrame = default;

            CurStamina = default;
            StaminaNextFrame = default;
            CurStack = default;

        }
    }
}