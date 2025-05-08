using System.Collections.Generic;
using System.Threading.Tasks;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using UnityEngine;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveSpawnBullet : IBuffActive
    {
        private readonly List<BuffSpawnBulletData> bullets;

        public BuffActiveSpawnBullet(BuffActiveData data)
        {
            this.bullets = data.Bullets;
        }
        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            var targetTm = target.GameObject.transform;
            var shooter = me;
            foreach (var bullet in bullets)
            {
                Spawn(bullet, targetTm, shooter, rootSource);
            }
        }

        private async void Spawn(BuffSpawnBulletData data, Transform tm, IEntity shooter, IEntity rootSource)
        {
            if (data.Frame > 0)
            {
                var delayMs = data.Frame * 1000 / CommonVariables.GameFrame;
                await Task.Delay(delayMs);
            }

            if (tm == null)
                return;

            var position = tm.position;
            var forward = tm.forward;

            var rot = Quaternion.LookRotation(forward, Vector3.up);
            var randomPosition = new Vector3(Random.Range(data.PositionRandomMin.x, data.PositionRandomMax.x),
                Random.Range(data.PositionRandomMin.y, data.PositionRandomMax.y),
                Random.Range(data.PositionRandomMin.z, data.PositionRandomMax.z));
            var pos = position + rot * (data.Position + randomPosition);

            var vel = BulletHelper.GetVelocity(data.Angle, data.AngleY, data.Speed, forward);
            var spawnBullet =
                new GameCommandSpawnBullet(data.BulletPrefab2, pos, vel, rootSource, rootSource.Team, data.ActionType, 0, 0);
            spawnBullet.Send();
        }
    }
}