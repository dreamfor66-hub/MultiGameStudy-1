using System.Collections.Generic;
using Photon.Bolt;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Character;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour]
    public class BoltBulletManager : MonoBehaviour
    {
        public static BoltBulletManager Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        public void OnDestroy()
        {
            Instance = null;
        }

        private readonly BulletSpawner spawner = new BulletSpawner();
        private readonly List<BulletBehaviour> bullets = new List<BulletBehaviour>();

        public void SpawnClient(int tableId, int uniqueId, BulletSpawnInfo spawnInfo)
        {
            var prefab = BulletTable.Instance.GetById(tableId);
            var bullet = spawner.Spawn(prefab);
            bullet.InitClient(spawnInfo);
            bullets.Add(bullet);
        }

        public void SpawnServer(int tableId, int uniqueId, BulletSpawnInfo spawnInfo, BulletServerInfo serverInfo)
        {
            var prefab = BulletTable.Instance.GetById(tableId);
            var bullet = spawner.Spawn(prefab);
            bullet.InitServer(spawnInfo, serverInfo);
            bullets.Add(bullet);
        }

        public void Hit(HitResultInfo hitInfo)
        {
            var attackerId = hitInfo.Main.Attacker?.EntityId ?? -1;
            var victimId = hitInfo.Main.Victim?.EntityId ?? -1;
            var attacker = bullets.Find(x => x.EntityId == attackerId);
            var victim = bullets.Find(x => x.EntityId == victimId);

            if (attacker != null)
            {
                attacker.Hit();

                if (hitInfo.Main.Victim is CharacterBehaviour)
                {
                    attacker.HitOnlyBody();
                }
                else if(victim != null)
                {
                    if (victim.bulletData.Dp.isUseDp())
                    {
                        attacker.HitOnlyBody();
                    }
                }
            }
            if (victim != null)
            {
                victim.Hurt(hitInfo);
            }
        }

        public void FixedUpdate()
        {
            foreach (var bullet in bullets)
            {
                bullet.UpdateFrame(BoltFrameSync.Frame);
                if (bullet.IsEnd)
                    bullet.Despawn();
            }
            bullets.RemoveAll(x => x.IsEnd);
        }
    }
}