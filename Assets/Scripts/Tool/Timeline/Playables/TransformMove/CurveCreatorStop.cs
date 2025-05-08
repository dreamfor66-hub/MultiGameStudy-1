using UnityEngine;

namespace Rogue.Tool.Timeline.Playables.TransformMove
{
    public class CurveCreatorStop
    {
        public CurveCreatorStop()
        {
        }

        public AnimationCurve Create(float duration)
        {
            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(duration, 0f, 0f, 0f));
            return curve;
        }
    }
}