using System;

namespace Rogue.Ingame.GameCommand
{

    public struct GameCommandNextStage : IGameCommand
    {
        public static void Send()
        {
            (new GameCommandNextStage()).SendCur();
        }

        private void SendCur()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Listen(Action<GameCommandNextStage> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandNextStage> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}