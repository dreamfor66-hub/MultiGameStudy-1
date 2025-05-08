using UnityEngine;

namespace Rogue.Tool.Timeline.Playables.TransformMove
{
    public class CurveCreatorFixedDistanceLinear
    {
        private float distance;
        public CurveCreatorFixedDistanceLinear(CurvePresetData data)
        {
            distance = data.Distance;
        }

        public AnimationCurve Create(float duration)
        {
            var curve = new AnimationCurve();
            var vel = distance / duration;
            curve.AddKey(new Keyframe(0f, 0f, vel, vel));
            curve.AddKey(new Keyframe(duration, distance, vel, vel));
            return curve;
        }
    }
}