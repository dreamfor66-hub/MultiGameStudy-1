using FMLib.Structs;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Rogue.Ingame.Input
{
    public static class DirectionEncoder
    {
        public static readonly int Resolution = 60;
        public static readonly float DeadZone = 0.3f;

        public static float UnitAngle => 360f / Resolution;

        public static int Encode(VectorXZ dir)
        {
            var mag = dir.Magnitude;
            if (mag < DeadZone)
                return 0;

            var angle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);
            angle += UnitAngle / 2f;

            if (angle < 0f)
                angle += 360f;

            return (int)(angle / UnitAngle) + 1;
        }

        public static VectorXZ Decode(int value)
        {
            if (value == 0)
                return VectorXZ.Zero;
            var angle = (value - 1) * UnitAngle;

            return new VectorXZ(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
        }

        public static float ToAngle(VectorXZ dir)
        {
            var angle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);
            if (angle < 0f)
                angle += 360f;
            return angle;
        }

        public static VectorXZ FromAngle(float value)
        {
            return new VectorXZ(Mathf.Sin(value * Mathf.Deg2Rad), Mathf.Cos(value * Mathf.Deg2Rad));
        }
    }
}