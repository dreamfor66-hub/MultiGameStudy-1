using UnityEngine;

namespace Rogue.Tool.Timeline.Playables.TransformMove
{
    public class CurveCreatorFixedDistanceStartEndVel
    {
        private readonly float distance;
        private readonly float startVel;
        private readonly float endVel;
        public CurveCreatorFixedDistanceStartEndVel(CurvePresetData data)
        {
            distance = data.Distance;
            startVel = data.StartVel;
            endVel = data.EndVel;
        }

        public AnimationCurve Create(float duration)
        {
            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, startVel, startVel));
            curve.AddKey(new Keyframe(duration, distance, endVel, endVel));
            return curve;
        }
    }
}