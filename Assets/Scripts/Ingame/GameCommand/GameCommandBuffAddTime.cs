using System;
using Rogue.Ingame.Buff;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandBuffAddTime : IGameCommand
    {
        public IEntity RootSource;
        public IEntity Target;
        public BuffIdentifer BuffId;
        public int TimeModifier;
        public bool IsDurationChanged;

        public GameCommandBuffAddTime(IEntity rootSource, IEntity target, BuffIdentifer buffId, int timeModifier, bool isDurationChanged)
        {
            RootSource = rootSource;
            Target = target;
            BuffId = buffId;
            TimeModifier = timeModifier;
            IsDurationChanged = isDurationChanged;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(IEntity rootSource, IEntity target, BuffIdentifer buffId, int timeModifier, bool isDurationChanged)
        {
            var cmd = new GameCommandBuffAddTime(rootSource, target, buffId, timeModifier, isDurationChanged);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandBuffAddTime> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandBuffAddTime> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}