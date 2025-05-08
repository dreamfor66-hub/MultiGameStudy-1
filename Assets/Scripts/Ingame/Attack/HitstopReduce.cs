using UnityEngine;

namespace Rogue.Ingame.Attack
{
    public static class HitstopReduce
    {
        private static readonly float[] Reduces = { 1f, .75f, .5f };

        public static float GetReduce(int prevHitCount)
        {
            var idx = Mathf.Clamp(prevHitCount, 0, Reduces.Length - 1);
            return Reduces[idx];
        }

        public static int GetIdx(int prevHitCount)
        {
            return Mathf.Clamp(prevHitCount, 0, Reduces.Length - 1);
        }
    }
}