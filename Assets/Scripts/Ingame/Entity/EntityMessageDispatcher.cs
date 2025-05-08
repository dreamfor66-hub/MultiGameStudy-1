using Rogue.Ingame.Event;
using System;

namespace Rogue.Ingame.Entity
{
    public interface IEntityMessage
    {
    }
    public class EntityMessageDispatcher
    {
        private readonly MessageMediator<IEntityMessage> eventHandler = new MessageMediator<IEntityMessage>();

        public void Send<T>(T evt) where T : IEntityMessage
        {
            eventHandler.Send(evt);
        }

        public void Listen<T>(Action<T> handler) where T : IEntityMessage
        {
            eventHandler.Listen(handler);
        }

        public void Remove<T>(Action<T> handler) where T : IEntityMessage
        {
            eventHandler.Remove(handler);
        }
    }
}
