using System;
namespace Rogue.Ingame.Event
{
    public static class EventDispatcher
    {
        private static MessageMediator<IEvent> mediator = new MessageMediator<IEvent>();

        public static void Listen<T>(Action<T> handler) where T : IEvent
        {
            mediator.Listen<T>(handler);
        }

        public static void Remove<T>(Action<T> handler) where T : IEvent
        {
            mediator.Remove<T>(handler);
        }

        public static void Send<T>(T evt) where T : IEvent
        {
            mediator.Send<T>(evt);
        }

    }
}