using System;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Rogue.Tool.Timeline.Playables.HitCollider
{
    [Serializable]
    public class HitColliderClip : PlayableAsset, ITimelineClipAsset
    {
        public HitColliderBehaviour template = new HitColliderBehaviour();

        public ClipCaps clipCaps => ClipCaps.None;
        public int GroupId;

        public override double duration
        {
            get
            {
                return CommonVariables.TimelineDefaultClipLength;
            }
        }

        [HideLabel]
        public HitboxInfo Info;

        public BuffData BuffData;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<HitColliderBehaviour>.Create(graph, template);
            var clone = playable.GetBehaviour();
            return playable;
        }
    }
}
