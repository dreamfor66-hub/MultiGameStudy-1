using System;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandAddBuff : IGameCommand
    {
        public IEntity RootSource;
        public IEntity Target;
        public BuffData BuffData;

        public GameCommandAddBuff(IEntity rootSource, IEntity target, BuffData buffData)
        {
            RootSource = rootSource;
            Target = target;
            BuffData = buffData;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(IEntity rootSource, IEntity target, BuffData buffData)
        {
            var cmd = new GameCommandAddBuff(rootSource, target, buffData);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandAddBuff> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandAddBuff> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}