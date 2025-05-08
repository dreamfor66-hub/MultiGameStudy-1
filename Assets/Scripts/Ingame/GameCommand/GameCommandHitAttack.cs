using Rogue.Ingame.Attack.Struct;
using System;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandHitAttack : IGameCommand
    {
        public HitInfo HitInfo;

        public GameCommandHitAttack(HitInfo hitInfo)
        {
            HitInfo = hitInfo;
        }

        public static void Send(HitInfo hitInfo)
        {
            var cmd = new GameCommandHitAttack(hitInfo);
            cmd.Send();
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Listen(Action<GameCommandHitAttack> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandHitAttack> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}