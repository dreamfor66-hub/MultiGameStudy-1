using FMLib.Structs;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using UnityEngine;
using BulletEvent = Rogue.Ingame.Data.BulletEvent;

namespace Rogue.Ingame.Bullet
{
    public class BulletSpawnBullet
    {
        private readonly Transform tm;
        private readonly IEntity entity;
        private readonly BulletSpawnBulletData data;
        private BulletServerInfo serverInfo;

        public BulletSpawnBullet(Transform tm, IEntity entity, BulletSpawnBulletData data)
        {
            this.tm = tm;
            this.entity = entity;
            this.data = data;
        }

        public void OnEvent(Data.BulletEvent evt)
        {
            if (!data.Enabled)
                return;
            foreach (var bullet in data.Bullets)
            {
                if (!CheckConditionEvent(bullet, evt))
                    continue;
                SpawnBullet(bullet);
            }
        }

        public void UpdateFrame(int frame)
        {
            if (!data.Enabled)
                return;
            foreach (var bullet in data.Bullets)
            {
                if (!CheckConditionFrame(bullet, frame))
                    continue;
                SpawnBullet(bullet);
            }
        }

        public void Reset(BulletServerInfo info)
        {
            this.serverInfo = info;
        }

        private void SpawnBullet(BulletSpawnSingleData bullet)
        {
            var prefab = bullet.Prefab;
            var position = tm.position;
            var velocity = BulletHelper.GetVelocity(bullet.Angle, bullet.AngleY, bullet.Speed, tm.forward);
            var rootSource = serverInfo.RootSource;
            var team = entity.Team;
            var actionType = serverInfo.ActionType;
            var cmd = new GameCommandSpawnBullet(prefab, position, velocity, rootSource, team, actionType, serverInfo.AdditionalCriticalChance, serverInfo.AdditionalCriticalDamagePercent);
            cmd.Send();
        }


        private bool CheckConditionEvent(BulletSpawnSingleData bullet, Data.BulletEvent evt)
        {
            return bullet.ConditionType == BulletSpawnBulletConditionType.Event && bullet.ConditionEvent == evt;
        }

        private bool CheckConditionFrame(BulletSpawnSingleData bullet, int frame)
        {
            return bullet.ConditionType == BulletSpawnBulletConditionType.Frame && bullet.ConditionFrame == frame;
        }
    }
}