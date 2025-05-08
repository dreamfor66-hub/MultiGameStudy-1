using System;
using UnityEngine;

namespace FMLib.Structs
{
    [System.Serializable]
    public struct VectorXZ : IEquatable<VectorXZ>
    {
        public float x;
        public float z;

        public static VectorXZ Zero => new VectorXZ(0f, 0f);

        public VectorXZ Normalized
        {
            get
            {
                var size = Magnitude;
                return size == 0 ? new VectorXZ(x, z) : new VectorXZ(x / size, z / size);
            }
        }

        public float Magnitude => Mathf.Sqrt(x * x + z * z);

        public VectorXZ(float x, float z)
        {
            this.x = x;
            this.z = z;
        }

        public VectorXZ(Vector3 vec)
        {
            this.x = vec.x;
            this.z = vec.z;
        }

        public static implicit operator Vector3(VectorXZ xz)
        {
            return new Vector3(xz.x, 0f, xz.z);
        }

        public bool Equals(VectorXZ other)
        {
            return x.Equals(other.x) && z.Equals(other.z);
        }

        public override bool Equals(object obj)
        {
            return obj is VectorXZ other && Equals(other);
        }

        public static bool operator ==(VectorXZ left, VectorXZ right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VectorXZ left, VectorXZ right)
        {
            return !left.Equals(right);
        }

        public static VectorXZ operator -(VectorXZ x)
        {
            return new VectorXZ(-x.x, -x.z);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x.GetHashCode() * 397) ^ z.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{x:0.00},{z:0.00}";
        }
    }
}
