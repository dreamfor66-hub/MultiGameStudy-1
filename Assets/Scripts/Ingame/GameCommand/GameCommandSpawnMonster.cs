using System;
using UnityEngine;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandSpawnMonster : IGameCommand
    {
        public GameObject Prefab;
        public Vector3 Position;

        public GameCommandSpawnMonster(GameObject prefab, Vector3 position)
        {
            Prefab = prefab;
            Position = position;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Send(GameObject prefab, Vector3 position)
        {
            var cmd = new GameCommandSpawnMonster(prefab, position);
            cmd.Send();
        }

        public static void Listen(Action<GameCommandSpawnMonster> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandSpawnMonster> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }

    }
}