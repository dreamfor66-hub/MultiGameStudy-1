using System;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandBuffRelease : IGameCommand
    {
        public IEntity RootSource;
        public IEntity Target;
        public BuffIdentifer BuffId;

        public GameCommandBuffRelease(IEntity rootSource, IEntity target, BuffIdentifer buffId)
        {
            RootSource = rootSource;
            Target = target;
            BuffId = buffId;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(IEntity rootSource, IEntity target, BuffIdentifer buffId)
        {
            var cmd = new GameCommandBuffRelease(rootSource, target, buffId);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandBuffRelease> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandBuffRelease> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}