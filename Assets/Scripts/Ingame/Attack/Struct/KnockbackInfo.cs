using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Attack.Struct
{
    public struct KnockbackInfo
    {
        public Vector3 Direction;
        public float Distance;
        public KnockbackStrength Strength;
        public int KnockStopFrame;
    }
}