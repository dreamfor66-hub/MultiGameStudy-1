using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Tool.Timeline.Playables.TransformMove
{
    public class CurveCreatorConstantVelocityEaseInOut
    {
        private readonly float vel;
        private readonly float easeInFrame;
        private readonly float startvel;
        private readonly float easeOutFrame;
        private readonly float endVel;

        public CurveCreatorConstantVelocityEaseInOut(CurvePresetData data)
        {
            vel = data.Vel;
            easeInFrame = data.EaseInFrame;
            startvel = data.StartVel;
            easeOutFrame = data.EaseOutFrame;
            endVel = data.EndVel;
        }

        private float InDuration => easeInFrame / CommonVariables.GameFrame;
        private float OutDuration => easeOutFrame / CommonVariables.GameFrame;

        private float InDistance => InDuration > 0 ? InCurve(InDuration) : 0;

        public AnimationCurve Create(float duration)
        {
            var curve = new AnimationCurve();
            float t = 0;
            if (InDuration > 0)
            {
                curve.AddKey(new Keyframe(0, 0, startvel, startvel));
            }

            t = InDuration;
            curve.AddKey(new Keyframe(t, Distance(t, duration), vel, vel));
            t = (float)duration - OutDuration;
            curve.AddKey(new Keyframe(t, Distance(t, duration), vel, vel));

            if (OutDuration > 0)
            {
                t = (float)duration;
                curve.AddKey(new Keyframe(t, Distance(t, duration), endVel, endVel));
            }

            return curve;
        }

        private float Distance(float t, float duration)
        {
            if (t < InDuration)
            {
                return InCurve(t);
            }
            else if (duration - t > OutDuration)
            {
                return InDistance + vel * (t - InDuration);
            }
            else
            {
                var middleDuration = (float)(duration - InDuration - OutDuration);
                return InDistance + vel * middleDuration + OutCurve(t - middleDuration - InDuration);
            }
        }

        private float InCurve(float t)
        {
            if (InDuration == 0) return 0;
            var a = (vel - startvel) / (2 * InDuration);
            var b = startvel;
            return a * t * t + b * t;
        }

        private float OutCurve(float t)
        {
            if (OutDuration == 0) return 0;
            var a = (endVel - vel) / (2 * OutDuration);
            var b = vel;
            return a * t * t + b * t;
        }
    }

}