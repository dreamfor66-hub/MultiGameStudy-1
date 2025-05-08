using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Rogue.Tool.Timeline.Playables.TransformMove
{
    public enum MoveDirection
    {
        XDir = 1,
        YDir,
        ZDir,
    }

    [TrackColor(0.2180046f, 0.6509434f, 0.4276668f)]
    [TrackClipType(typeof(TransformMoveClip))]
    [TrackBindingType(typeof(Transform))]
    public class TransformMoveTrack : TrackAsset
    {
        public MoveDirection MoveDirection = MoveDirection.ZDir;
        public float StartPos;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var clip in GetClips())
            {
                if (clip.asset is TransformMoveClip moveClip)
                {
                    moveClip.StartTime = clip.start;
                    moveClip.EndTime = clip.end;
                }
            }

            var playable = ScriptPlayable<TransformMoveMixerBehaviour>.Create(graph, inputCount);
            var behaviour = playable.GetBehaviour();
            behaviour.Direction = MoveDirection;
            behaviour.StartPos = StartPos;
            return playable;
        }
    }
}
