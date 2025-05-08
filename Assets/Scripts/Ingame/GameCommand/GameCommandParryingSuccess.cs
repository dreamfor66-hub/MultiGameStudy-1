using System;
using Rogue.Ingame.Attack.Struct;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandParryingSuccess : IGameCommand
    {
        public ParryingInfo ParryingInfo;

        public GameCommandParryingSuccess(ParryingInfo parryingInfo)
        {
            ParryingInfo = parryingInfo;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(ParryingInfo info)
        {
            var cmd = new GameCommandParryingSuccess(info);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandParryingSuccess> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandParryingSuccess> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}