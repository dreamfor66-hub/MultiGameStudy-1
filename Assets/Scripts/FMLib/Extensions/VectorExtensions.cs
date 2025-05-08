using FMLib.Structs;
using UnityEngine;

namespace FMLib.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ToVec3XZ(this Vector2 vec2)
        {
            return new Vector3(vec2.x, 0f, vec2.y);
        }

        public static VectorXZ ToVecXZ(this Vector2 vec2)
        {
            return new VectorXZ(vec2.x, vec2.y);
        }

        public static Vector3 ToVec3XZ(this Vector3 vec3)
        {
            vec3.y = 0f;
            return vec3;
        }

        public static Vector2 Rotate(this Vector2 vec, float radian)
        {
            var cos = Mathf.Cos(radian);
            var sin = Mathf.Sin(radian);
            return new Vector2(
                vec.x * cos - vec.y * sin,
                vec.y * sin + vec.y * cos
            );
        }

        public static Vector3 Random(Vector3 min, Vector3 max)
        {
            return new Vector3(UnityEngine.Random.Range(min.x, max.x),
                UnityEngine.Random.Range(min.y, max.y),
                UnityEngine.Random.Range(min.z, max.z));
        }
    }
}
