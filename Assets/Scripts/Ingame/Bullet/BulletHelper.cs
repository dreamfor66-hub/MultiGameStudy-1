using UnityEngine;

namespace Rogue.Ingame.Bullet
{
    public static class BulletHelper
    {
        public static Vector3 GetVelocity(float angle, float angleY, float speed, Vector3 forward)
        {
            forward.y = 0f;
            forward = forward == Vector3.zero ? Vector3.forward : forward.normalized;
            var left = Vector3.Cross(forward, Vector3.up);
            return Quaternion.AngleAxis(angle, Vector3.up) * (Quaternion.AngleAxis(angleY, left) * forward) * speed;
        }
    }
}
