using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Attack
{
    public static class KnockbackCalculator
    {
        private static readonly float basisDist = 1f;
        private static readonly float basisTime = 0.2f;
        private static readonly float power = 2.3f;
        private static readonly float Gravity = 49f;
        private static readonly float coEff;

        static KnockbackCalculator()
        {
            coEff = basisDist / Mathf.Pow(basisTime, power);
        }

        public static float TotalTime(float distance)
        {
            return Mathf.Pow(distance / coEff, 1 / power);
        }

        public static float TimeToDist(float maxDist, float time)
        {
            var totalTime = TotalTime(maxDist);
            time = Mathf.Clamp(time, 0f, totalTime);
            return -coEff * Mathf.Pow(Mathf.Abs(time - totalTime), power) + maxDist;
        }

        public static float TimeToHeight(float maxDist, float time)
        {
            var totalTime = TotalTime(maxDist);
            var height = -Gravity * time * time / 2f + Gravity * totalTime * time / 2f;
            return Mathf.Clamp(height, 0f, float.MaxValue);
        }

        public static float TotalFrame(float distance)
        {
            return TotalTime(distance) * CommonVariables.GameFrame;
        }

        public static float FrameToDist(float maxDist, float frame)
        {
            return TimeToDist(maxDist, (float)frame / CommonVariables.GameFrame);
        }

        public static float FrameToHeight(float maxDist, float frame)
        {
            return TimeToHeight(maxDist, (float)frame / CommonVariables.GameFrame);
        }

        public static bool IsToGetup(KnockbackStrength strength)
        {
            return strength == KnockbackStrength.Mid || strength == KnockbackStrength.High;
        }

    }
}