using System;
using Rogue.Ingame.Attack.Struct;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandShield : IGameCommand
    {
        public ShieldInfo ShieldInfo;

        public GameCommandShield(ShieldInfo shieldInfo)
        {
            ShieldInfo = shieldInfo;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(ShieldInfo shieldInfo)
        {
            var cmd = new GameCommandShield(shieldInfo);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandShield> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandShield> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}