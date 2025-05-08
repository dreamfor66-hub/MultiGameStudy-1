using System;
using Rogue.Ingame.Data;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Rogue.Tool.Timeline.Playables.CurvedAnimator
{
    [Serializable]
    public class CurvedAnimatorClip : PlayableAsset, ITimelineClipAsset
    {
        public override double duration
        {
            get
            {
                if (template.Curve != null && template.Curve.length > 0)
                    return template.Curve[template.Curve.length - 1].time / CommonVariables.GameFrame;
                else if (template.Clip != null)
                    return template.Clip.length;
                else
                    return CommonVariables.TimelineDefaultClipLength;
            }
        }

        public CurvedAnimatorBehaviour template = new CurvedAnimatorBehaviour();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CurvedAnimatorBehaviour>.Create(graph, template);
            var clone = playable.GetBehaviour();
            return playable;
        }
    }
}
