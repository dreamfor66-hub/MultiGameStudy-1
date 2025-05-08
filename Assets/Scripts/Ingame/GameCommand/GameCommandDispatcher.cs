using System;
using Rogue.Ingame.Event;

namespace Rogue.Ingame.GameCommand
{
    public interface IGameCommand
    {
    }

    public class GameCommandDispatcher
    {
        private static readonly MessageMediator<IGameCommand> mediator = new MessageMediator<IGameCommand>();

        public static void Send<T>(T cmd) where T : IGameCommand
        {
            mediator.Send(cmd);
        }

        public static void Listen<T>(Action<T> handler) where T : IGameCommand
        {
            mediator.Listen(handler);
        }

        public static void Remove<T>(Action<T> handler) where T : IGameCommand
        {
            mediator.Remove(handler);
        }
    }
}
