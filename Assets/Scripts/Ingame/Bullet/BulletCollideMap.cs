using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Bullet
{
    public class BulletCollideMap
    {
        private readonly BulletBehaviour bulletBehaviour;
        private readonly Transform tm;
        private Vector3 prevPosition;

        public BulletCollideMap(BulletBehaviour bulletBehaviour)
        {
            this.bulletBehaviour = bulletBehaviour;
            tm = bulletBehaviour.transform;
        }

        public void OnEvent(BulletEvent evt)
        {
            if (evt == BulletEvent.Init)
                prevPosition = tm.position;
        }

        public void UpdateFrame(int elapsedFrame)
        {
            var curPosition = tm.position;
            var diff = curPosition - prevPosition;
            var dir = diff.normalized;
            var mag = diff.magnitude;
            if (elapsedFrame > 0 && Physics.Raycast(prevPosition, dir, mag, LayerHelper.MapMask))
            {
                bulletBehaviour.CollideMap();
            }

            prevPosition = curPosition;
        }
    }
}