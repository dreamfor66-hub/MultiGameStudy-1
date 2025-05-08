using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Rogue.Ingame.Data;

namespace Rogue.Tool.Timeline.Playables.CharacterStatus
{
    [Serializable]
    public class CharacterStatusGhostClip : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.None;

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