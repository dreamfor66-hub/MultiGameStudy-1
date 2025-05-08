using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Attack
{
    public class HitstopCalculator
    {
        private readonly AnimationCurve curve;
        private readonly float length;
        private readonly float maxTimeInCurve;
        private float timeInCurve;

        public bool End => timeInCurve > maxTimeInCurve;
        public float CurSpeed => curve.Evaluate(timeInCurve);


        public HitstopCalculator(AnimationCurve curve, float length)
        {
            this.curve = curve;
            this.length = length;
            maxTimeInCurve = curve.keys[curve.keys.Length - 1].time;
            timeInCurve = 0f;
        }

        public float Elapse(float deltaTime)
        {
            timeInCurve += deltaTime / length;
            if (timeInCurve > maxTimeInCurve)
                return deltaTime;
            else
                return deltaTime * CurSpeed;
        }


    }
}