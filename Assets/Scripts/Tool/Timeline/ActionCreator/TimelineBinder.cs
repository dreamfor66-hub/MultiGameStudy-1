using Rogue.Tool.Timeline.Playables.CurvedAnimator;
using Rogue.Tool.Timeline.Playables.TransformMove;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Rogue.Tool.Timeline.ActionCreator
{
    public static class TimelineBinder
    {
        public static PlayableDirector BindTimeline(GameObject characterObj, TimelineAsset timelineAsset)
        {
            var playableDirector = characterObj.AddComponent<PlayableDirector>();
            playableDirector.playableAsset = timelineAsset;

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is CurvedAnimatorTrack)
                    playableDirector.SetGenericBinding(track, characterObj.GetComponent<Animator>());
                if (track is TransformMoveTrack)
                    playableDirector.SetGenericBinding(track, characterObj.transform);
            }

            return playableDirector;
        }
    }
}