using System;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandAddStack : IGameCommand
    {
        public IEntity Target;
        public int Count;

        public GameCommandAddStack(IEntity target, int count)
        {
            Target = target;
            Count = count;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(IEntity target, int count)
        {
            var cmd = new GameCommandAddStack(target, count);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandAddStack> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandAddStack> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}