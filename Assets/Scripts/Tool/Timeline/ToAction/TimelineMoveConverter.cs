using Rogue.Tool.Timeline.Playables.TransformMove;
using UnityEngine;

namespace Rogue.Tool.Timeline.ToAction
{
    public class TimelineMoveConverter
    {
        private AnimationCurve resultCurve = new AnimationCurve();
        private float startPos;

        private void Clear()
        {
            resultCurve = new AnimationCurve();
        }

        public AnimationCurve GetMoveCurve(TransformMoveTrack track)
        {
            Clear();
            startPos = track.StartPos;
            foreach (var clip in track.GetClips())
            {
                var moveClip = clip.asset as TransformMoveClip;
                AddClipCurve(moveClip);
            }

            return resultCurve;
        }

        private void AddClipCurve(TransformMoveClip moveClip)
        {
            var start = (float)moveClip.StartTime;
            var end = (float)moveClip.EndTime;
            var duration = end - start;
            var moveOffset = CurMove;
            var curve = moveClip.template.CalculateCurve(duration);
            var shareLastKey = CurTime != 0f && Mathf.RoundToInt(CurTime * 60) == Mathf.RoundToInt(start * 60);

            var firstKey = true;
            foreach (var key in curve.keys)
            {
                if (shareLastKey && firstKey)
                {
                    ChangeLastOutTangent(key.outTangent);
                }
                else
                {
                    var inTangent = firstKey ? 0 : key.inTangent;
                    var newKey = new Keyframe(key.time + start, key.value + moveOffset, inTangent, key.outTangent);
                    resultCurve.AddKey(newKey);
                }
                firstKey = false;
            }

            ChangeLastOutTangent(0);
        }

        private int LastIdx => resultCurve.keys.Length - 1;
        private Keyframe LastKey => resultCurve.keys[LastIdx];
        private float CurMove => resultCurve.keys.Length > 0 ? LastKey.value : startPos;
        private float CurTime => resultCurve.keys.Length > 0 ? LastKey.time : 0f;


        private void ChangeLastOutTangent(float outTangent)
        {
            var newKey = LastKey;
            newKey.outTangent = outTangent;
            resultCurve.MoveKey(LastIdx, newKey);
        }
    }
}