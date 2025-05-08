using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Rogue.Ingame.Data;

namespace Rogue.Tool.Timeline.Playables.TransformMove
{
    [Serializable]
    public class TransformMoveClip : PlayableAsset, ITimelineClipAsset
    {
        [HideLabel]
        public TransformMoveBehaviour template = new TransformMoveBehaviour();

        public ClipCaps clipCaps => ClipCaps.None;

        public double StartTime { get; set; }
        public double EndTime { get; set; }

        public override double duration
        {
            get
            {
                return CommonVariables.TimelineDefaultClipLength;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TransformMoveBehaviour>.Create(graph, template);
            var behaviour = playable.GetBehaviour();
            behaviour.StartTime = StartTime;
            behaviour.EndTime = EndTime;
            return playable;
        }


        [Title("Preview")]
        public AnimationCurve Curve;

        [Button]
        public void CalculateCurve()
        {
            Curve = template.CalculateCurve((float)(EndTime - StartTime));
        }
    }
}
