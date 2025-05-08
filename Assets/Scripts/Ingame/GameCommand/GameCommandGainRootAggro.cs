using System;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandGainRootAggro : IGameCommand
    {
        public IEntity Attacker;
        public IEntity Defender;
        public float Value;

        public GameCommandGainRootAggro(IEntity attacker, IEntity defender, float value)
        {
            Attacker = attacker;
            Defender = defender;
            Value = value;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(IEntity target, IEntity rootSource, float value)
        {
            var cmd = new GameCommandGainRootAggro(target, rootSource, value);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandGainRootAggro> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandGainRootAggro> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}