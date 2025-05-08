using System;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Core;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Util.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rogue.Ingame.Bullet
{
    public struct BulletSpawnInfo
    {
        public int EntityId;
        public Vector3 Position;
        public Vector3 Velocity;
        public int Frame;
        public int RootSourceId;
    }

    public struct BulletServerInfo
    {
        public Team Team;
        public IEntity RootSource;
        public ActionTypeMask ActionType;
        public int AdditionalCriticalChance;
        public int AdditionalCriticalDamagePercent;
    }

    public class BulletBehaviour : MonoBehaviour, ISpawnable, IEntity
    {
        public int EntityId => Info.EntityId;
        public GameObject GameObject => gameObject;
        public Team Team { get; private set; }

        public BulletSpawnInfo Info { get; private set; }
        public BulletServerInfo ServerInfo { get; private set; }
        public bool IsEnd { get; private set; }
        private int despawnAfter = -1;

        [SerializeField] public BulletData bulletData;

        private Action despawnAction;

        private BulletAttack bulletAttack;
        private BulletVfx bulletVfx;
        private BulletSpawnBullet bulletSpawnBullet;
        private BulletCollideMap bulletCollideMap;
        private BulletMove bulletMove;
        private DpModule dpModule;

        public void Awake()
        {
            SceneManager.activeSceneChanged += (Scene scene, Scene scene2) => { IsEnd = true; };
            bulletMove = new BulletMove(transform, bulletData);
            bulletAttack = new BulletAttack(transform, this, bulletData.Attack);
            bulletVfx = new BulletVfx(transform, bulletData.Vfx);
            bulletSpawnBullet = new BulletSpawnBullet(transform, this, bulletData.SpawnBullet);
            bulletCollideMap = new BulletCollideMap(this);
            dpModule = new DpModule(bulletData.Dp);
            var hurtboxes = GetComponentsInChildren<Hurtbox>(true);
            foreach (var hurtbox in hurtboxes)
                hurtbox.SetEntity(this);
        }

        public void InitClient(BulletSpawnInfo spawnInfo)
        {
            Clear();
            this.Info = spawnInfo;
            IsEnd = false;
            bulletMove.Init(spawnInfo);
            dpModule.Reset();
            UpdateFrame(0);
            OnEvent(BulletEvent.Init);
        }

        public void InitServer(BulletSpawnInfo spawnInfo, BulletServerInfo serverInfo)
        {
            Clear();
            this.Info = spawnInfo;
            this.ServerInfo = serverInfo;
            Team = bulletData.Common.IsTeamFixed ? bulletData.Common.FixedTeam : serverInfo.Team;
            IsEnd = false;
            bulletMove.Init(spawnInfo);
            bulletAttack.Reset(serverInfo);
            bulletSpawnBullet.Reset(serverInfo);
            dpModule.Reset();
            UpdateFrame(0);
            OnEvent(BulletEvent.Init);
        }

        public void OnEvent(Data.BulletEvent evt)
        {
            bulletAttack.OnEvent(evt);
            bulletVfx.OnEvent(evt);
            bulletSpawnBullet.OnEvent(evt);
            bulletCollideMap.OnEvent(evt);
            if (bulletData.Common.DespawnBy.Contains(evt) && despawnAfter <= 0)
                despawnAfter = bulletData.Common.DespawnDelayFrame;
        }

        public void Hit()
        {
            OnEvent(Data.BulletEvent.Hit);
        }

        public void HitOnlyBody()
        {
            OnEvent(Data.BulletEvent.HitOnlyBody);
        }

        public void Hurt(HitResultInfo hitInfo)
        {
            OnEvent(Data.BulletEvent.Hurt);
            if (dpModule.Hurt(hitInfo))
                OnEvent(BulletEvent.DpDead);
        }

        public void CollideMap()
        {
            OnEvent(Data.BulletEvent.CollideMap);
        }

        private void Clear()
        {
            hitstopFrame = 0;
            delayedFrame = 0;
            despawnAfter = -1;
        }

        private int hitstopFrame;
        private int delayedFrame;
        public void Hitstop()
        {
            if (bulletData.Attack.HitstopFrame > 0)
                hitstopFrame = bulletData.Attack.HitstopFrame;
        }

        public void UpdateFrame(int frame)
        {
            if (hitstopFrame > 0)
            {
                hitstopFrame--;
                delayedFrame++;
                return;
            }

            var elapsedFrame = Mathf.Clamp(frame - Info.Frame - delayedFrame, 0, bulletData.Common.LifeTimeFrame);

            bulletMove.UpdateFrame(elapsedFrame);
            bulletAttack.UpdateFrame(elapsedFrame);
            bulletSpawnBullet.UpdateFrame(elapsedFrame);
            bulletCollideMap.UpdateFrame(elapsedFrame);

            if (despawnAfter >= 0)
            {
                if (--despawnAfter <= 0)
                    IsEnd = true;
            }

            if (elapsedFrame >= bulletData.Common.LifeTimeFrame)
                IsEnd = true;
        }

        public void Despawn()
        {
            despawnAction();
        }

        public void RegisterDespawn(Action action)
        {
            despawnAction = action;
        }

        public void OnSpawn()
        {
            EntityTable.Add(this);
            AttackCalculator.Add(bulletAttack);
        }

        public void OnDespawn()
        {
            OnEvent(Data.BulletEvent.Despawn);
            bulletAttack.OnDespawn();
            EntityTable.Remove(this);
            AttackCalculator.Remove(bulletAttack);
        }

        public void End()
        {
            IsEnd = true;
        }

        private void OnDestroy()
        {
            Despawn();
        }
    }
}
