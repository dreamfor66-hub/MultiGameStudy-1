using UnityEngine;

namespace FMLib.Curve
{
    public static class CurveGenerator
    {
        public static AnimationCurve Identity(float maxTime)
        {
            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 1f, 1f));
            curve.AddKey(new Keyframe(maxTime, maxTime, 1f, 1f));
            return curve;
        }
    }
}