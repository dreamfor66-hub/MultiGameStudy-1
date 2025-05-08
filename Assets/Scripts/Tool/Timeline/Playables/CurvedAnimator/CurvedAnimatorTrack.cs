using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Rogue.Tool.Timeline.Playables.CurvedAnimator
{
    [TrackColor(0f, 0.5f, 1f)]
    [TrackClipType(typeof(CurvedAnimatorClip))]
    [TrackBindingType(typeof(Animator))]
    public class CurvedAnimatorTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<CurvedAnimatorMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
