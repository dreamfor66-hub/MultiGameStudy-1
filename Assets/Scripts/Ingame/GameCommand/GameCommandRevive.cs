using System;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandRevive : IGameCommand
    {
        public IEntity Target;
        public int HpPercent;

        public GameCommandRevive(IEntity target, int hpPercent)
        {
            Target = target;
            HpPercent = hpPercent;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(IEntity target, int hpPercent)
        {
            var cmd = new GameCommandRevive(target, hpPercent);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandRevive> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandRevive> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}