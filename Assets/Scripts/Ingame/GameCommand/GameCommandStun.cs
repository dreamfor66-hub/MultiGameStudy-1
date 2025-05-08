using System;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandStun : IGameCommand
    {
        public IEntity Target;
        public int Frame;

        public GameCommandStun(IEntity target, int frame)
        {
            Target = target;
            Frame = frame;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(IEntity target, int frame)
        {
            var cmd = new GameCommandStun(target, frame);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandStun> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandStun> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}