using FMLib.Structs;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Input;
using UnityEngine;

namespace Rogue.Ingame.Network
{
    public interface IDataFrame
    {
        public int ServerFrame { get; }
    }

    public readonly struct InputFrame : IDataFrame
    {
        public int ServerFrame => RealFrame + InputDelay;

        public readonly InputState Input;
        public readonly int RealFrame;
        public readonly int InputDelay;

        public InputFrame(InputState input, int realFrame, int inputDelay)
        {
            Input = input;
            RealFrame = realFrame;
            InputDelay = inputDelay;
        }

        public override string ToString()
        {
            return $"[{RealFrame}+{InputDelay}]({Input.Direction}/{Input.Buttons})";
        }
    }

    public readonly struct StateFrame : IDataFrame
    {
        public int ServerFrame { get; }
        public readonly CharacterTotalInfo TotalInfo;

        public StateFrame(CharacterTotalInfo info, int serverFrame)
        {
            TotalInfo = info;
            ServerFrame = serverFrame;
        }
    }

    public readonly struct HitstopFrame : IDataFrame
    {
        public int ServerFrame { get; }
        public readonly HitstopInfo HitstopInfo;

        public HitstopFrame(HitstopInfo hitstopInfo, int serverFrame)
        {
            HitstopInfo = hitstopInfo;
            ServerFrame = serverFrame;
        }
    }

    public readonly struct SetDirectionFrame : IDataFrame
    {
        public int ServerFrame { get; }
        public readonly VectorXZ Direction;

        public SetDirectionFrame(VectorXZ direction, int serverFrame)
        {
            Direction = direction;
            ServerFrame = serverFrame;
        }
    }

    public readonly struct HurtFrame : IDataFrame
    {
        public int ServerFrame { get; }
        public readonly KnockbackInfo Knockback;
        public readonly bool IsDie;

        public HurtFrame(KnockbackInfo knockback, int frame, bool isDie)
        {
            Knockback = knockback;
            ServerFrame = frame;
            IsDie = isDie;
        }
    }

    public readonly struct StunFrame : IDataFrame
    {
        public int ServerFrame { get; }
        public readonly int Frame;

        public StunFrame(int serverFrame, int frame)
        {
            ServerFrame = serverFrame;
            Frame = frame;
        }
    }

    public readonly struct ForceActionFrame : IDataFrame
    {
        public int ServerFrame { get; }
        public readonly int ActionIdx;

        public ForceActionFrame(int serverFrame, int actionIdx)
        {
            ServerFrame = serverFrame;
            ActionIdx = actionIdx;
        }
    }

    public readonly struct ReviveFrame : IDataFrame
    {
        public int ServerFrame { get; }

        public ReviveFrame(int serverFrame)
        {
            ServerFrame = serverFrame;
        }
    }

    public readonly struct ResetPositionFrame : IDataFrame
    {
        public int ServerFrame { get; }
        public readonly Vector3 Position;

        public ResetPositionFrame(Vector3 position, int serverFrame)
        {
            Position = position;
            ServerFrame = serverFrame;
        }
    }

    public readonly struct AddStackFrame : IDataFrame
    {
        public int ServerFrame { get; }
        public readonly int Count;

        public AddStackFrame(int count, int serverFrame)
        {
            Count = count;
            ServerFrame = serverFrame;
        }
    }
}