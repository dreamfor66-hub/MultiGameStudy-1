using System;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandBuffAddStack : IGameCommand
    {
        public IEntity RootSource;
        public IEntity Target;
        public BuffIdentifer BuffId;
        public int Value;

        public GameCommandBuffAddStack(IEntity rootSource, IEntity target, BuffIdentifer buffId, int value)
        {
            RootSource = rootSource;
            Target = target;
            BuffId = buffId;
            Value = value;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(IEntity rootSource, IEntity target, BuffIdentifer buffId, int value)
        {
            var cmd = new GameCommandBuffAddStack(rootSource, target, buffId, value);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandBuffAddStack> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandBuffAddStack> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}