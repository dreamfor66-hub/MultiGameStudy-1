using System.Collections.Generic;
using Photon.Bolt;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public class BoltSpawnEventServer : MonoBehaviour
    {
        public static BoltSpawnEventServer Instance { get; private set; }

        void Awake()
        {
            Instance = this;
            GameCommandSpawnBullet.Listen(OnSpawnBullet);
            GameCommandSpawnMonster.Listen(OnSpawnMonster);
        }

        void OnDestroy()
        {
            Instance = null;
            GameCommandSpawnBullet.Remove(OnSpawnBullet);
            GameCommandSpawnMonster.Remove(OnSpawnMonster);
        }

        private readonly Queue<GameCommandSpawnBullet> bulletQueue = new Queue<GameCommandSpawnBullet>();
        private readonly Queue<GameCommandSpawnMonster> monsterQueue = new Queue<GameCommandSpawnMonster>();

        public void SpawnAll()
        {
            while (monsterQueue.Count > 0)
            {
                var spawn = monsterQueue.Dequeue();
                SpawnMonster(spawn.Prefab, spawn.Position);
            }
            while (bulletQueue.Count > 0)
            {
                var spawn = bulletQueue.Dequeue();
                var uniqueId = EntityTable.GetId();
                var frame = BoltNetwork.ServerFrame + 1;
                SendEvent(spawn, uniqueId, frame);
                SpawnBullet(spawn, uniqueId, frame);
            }
        }

        private void OnSpawnBullet(GameCommandSpawnBullet cmd)
        {
            bulletQueue.Enqueue(cmd);
        }

        private void OnSpawnMonster(GameCommandSpawnMonster cmd)
        {
            monsterQueue.Enqueue(cmd);
        }

        private void SpawnMonster(GameObject prefab, Vector3 position)
        {
            var rotation = Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.up);
            BoltNetwork.Instantiate(prefab, position, rotation);
        }

        private void SendEvent(GameCommandSpawnBullet cmd, int entityId, int frame)
        {
            var spawnBulletEvent = SpawnBulletEvent.Create(GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
            spawnBulletEvent.UniqueId = entityId;
            spawnBulletEvent.TableId = BulletTable.Instance.GetId(cmd.Prefab);
            spawnBulletEvent.Position = cmd.Position;
            spawnBulletEvent.Velocity = cmd.Velocity;
            spawnBulletEvent.ServerFrame = frame;
            spawnBulletEvent.RootSourceId = cmd.RootSource.EntityId;
            spawnBulletEvent.Send();
        }

        private void SpawnBullet(GameCommandSpawnBullet cmd, int entityId, int frame)
        {
            var prefabId = BulletTable.Instance.GetId(cmd.Prefab);
            var spawnInfo = new BulletSpawnInfo
            {
                EntityId = entityId,
                Position = cmd.Position,
                Velocity = cmd.Velocity,
                Frame = frame,
                RootSourceId = cmd.RootSource.EntityId
            };
            var serverInfo = new BulletServerInfo
            {
                Team = cmd.Team,
                RootSource = cmd.RootSource,
                ActionType = cmd.ActionType,
                AdditionalCriticalChance = cmd.AdditionalCriticalChance,
                AdditionalCriticalDamagePercent = cmd.AdditionalCriticalDamagePercent,
            };
            BoltBulletManager.Instance.SpawnServer(prefabId, entityId, spawnInfo, serverInfo);
        }

    }
}