using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Bullet
{
    public class BulletMove
    {
        private readonly Transform tm;
        private readonly BulletData bulletData;

        private BulletSpawnInfo info;

        private int curFrame;
        private Vector3 curVelocity;
        private Vector3 curPosition;

        public BulletMove(Transform tm, BulletData bulletData)
        {
            this.tm = tm;
            this.bulletData = bulletData;
        }


        public void Init(BulletSpawnInfo info)
        {
            this.info = info;
            curFrame = 0;
            curVelocity = info.Velocity;
            curPosition = info.Position;
        }


        public void UpdateFrame(int frame)
        {
            var deltaTime = 1f / CommonVariables.GameFrame;
            while (curFrame < frame)
            {
                curFrame++;
                if (bulletData.MoveParabola.Enabled)
                {
                    var g = bulletData.MoveParabola.GravityAccel;
                    var resist = bulletData.MoveParabola.Resistance;
                    curVelocity += Vector3.down * g * deltaTime;
                    curVelocity += -curVelocity * resist * deltaTime;
                }

                curPosition += curVelocity * deltaTime;
            }

            tm.position = curPosition;
            if (curVelocity != Vector3.zero)
                tm.forward = curVelocity.normalized;
        }
    }
}