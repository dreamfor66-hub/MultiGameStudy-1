using System;
using Rogue.Ingame.Attack.Struct;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandHeal : IGameCommand
    {
        public HealInfo HealInfo;

        public GameCommandHeal(HealInfo healInfo)
        {
            HealInfo = healInfo;
        }

        public static void Send(HealInfo healInfo)
        {
            var cmd = new GameCommandHeal(healInfo);
            cmd.Send();
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Listen(Action<GameCommandHeal> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandHeal> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }

    }
}