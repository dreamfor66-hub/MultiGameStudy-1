using System;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandGainStatus : IGameCommand
    {
        public IEntity Target;

        public CharacterStatusType Type;
        public int Frame;

        public GameCommandGainStatus(IEntity target, CharacterStatusType type, int frame)
        {
            Target = target;
            Type = type;
            Frame = frame;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(IEntity target, CharacterStatusType type, int frame)
        {
            var cmd = new GameCommandGainStatus(target, type, frame);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandGainStatus> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandGainStatus> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}