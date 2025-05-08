using System;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.GameCommand
{
    public struct GameCommandSpawnBullet : IGameCommand
    {
        public BulletBehaviour Prefab;
        public Vector3 Position;
        public Vector3 Velocity;
        public IEntity RootSource;
        public Team Team;
        public ActionTypeMask ActionType;

        public int AdditionalCriticalChance;
        public int AdditionalCriticalDamagePercent;

        public GameCommandSpawnBullet(BulletBehaviour prefab, Vector3 position, Vector3 velocity, IEntity rootSource, Team team, ActionTypeMask actionType, int additionalCriticalChance, int additionalCriticalDamagePercent)
        {
            Prefab = prefab;
            Position = position;
            Velocity = velocity;
            RootSource = rootSource;
            Team = team;
            ActionType = actionType;
            AdditionalCriticalChance = additionalCriticalChance;
            AdditionalCriticalDamagePercent = additionalCriticalDamagePercent;
        }

        public void Send()
        {
            GameCommandDispatcher.Send(this);
        }

        public static void Listen(Action<GameCommandSpawnBullet> handler)
        {
            GameCommandDispatcher.Listen(handler);
        }

        public static void Remove(Action<GameCommandSpawnBullet> handler)
        {
            GameCommandDispatcher.Remove(handler);
        }
    }
}