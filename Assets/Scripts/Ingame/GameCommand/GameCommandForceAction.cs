using System;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandForceAction : IGameCommand
    {
        public IEntity Entity;
        public ActionData ActionData;

        public GameCommandForceAction(IEntity entity, ActionData actionData)
        {
            Entity = entity;
            ActionData = actionData;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(IEntity entity, ActionData actionData)
        {
            var cmd = new GameCommandForceAction(entity, actionData);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandForceAction> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandForceAction> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}