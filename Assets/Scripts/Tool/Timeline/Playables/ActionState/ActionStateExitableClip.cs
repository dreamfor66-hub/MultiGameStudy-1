using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Rogue.Ingame.Data;

namespace Rogue.Tool.Timeline.Playables.ActionState
{
    [Serializable]
    public class ActionStateExitableClip : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.All;

        public override double duration
        {
            get
            {
                return CommonVariables.TimelineDefaultClipLength;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return new Playable();
        }
    }
}