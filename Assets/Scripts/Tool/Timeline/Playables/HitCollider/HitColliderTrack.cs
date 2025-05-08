using Rogue.Ingame.Attack;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Rogue.Tool.Timeline.Playables.HitCollider
{
    [TrackColor(0.9716981f, 0.1541872f, 0.08708613f)]
    [TrackClipType(typeof(HitColliderClip))]
    [TrackBindingType(typeof(HitColliderAnchor))]
    public class HitColliderTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<HitColliderMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
