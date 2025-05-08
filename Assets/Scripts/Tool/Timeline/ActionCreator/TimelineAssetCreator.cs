using Rogue.Tool.Timeline.Playables.ActionState;
using Rogue.Tool.Timeline.Playables.CurvedAnimator;
using Rogue.Tool.Timeline.Playables.HitCollider;
using Rogue.Tool.Timeline.Playables.TransformMove;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Rogue.Tool.Timeline.ActionCreator
{
    public static class TimelineAssetCreator
    {
        public static TimelineAsset CreateTimelineAsset(string path, AnimationClip clip, TimelineTrackOptions options)
        {
            var timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();
            AssetDatabase.CreateAsset(timelineAsset, path);

            AddAnimatorTrack(timelineAsset, clip);
            AddMoveTrack(timelineAsset);
            AddActionStateTrack(timelineAsset);
            if (options.CreateHitColliderTrackGroup) AddHitColliderTrackGroup(timelineAsset);
            if (options.CreateVfxTrackGroup) AddVfxTrackGroup(timelineAsset);

            return timelineAsset;
        }

        private static void AddAnimatorTrack(TimelineAsset timelineAsset, AnimationClip clip)
        {
            var animatorTrack = timelineAsset.CreateTrack<CurvedAnimatorTrack>();
            var timelineClip = animatorTrack.CreateClip<CurvedAnimatorClip>();
            var animatorClip = timelineClip.asset as CurvedAnimatorClip;
            animatorClip.template.Clip = clip;
            animatorClip.template.ResetCurve();
            timelineClip.start = 0;
            timelineClip.duration = (double)clip.length;
        }

        private static void AddMoveTrack(TimelineAsset timelineAsset)
        {
            timelineAsset.CreateTrack<TransformMoveTrack>();
        }

        private static void AddActionStateTrack(TimelineAsset timelineAsset)
        {
            timelineAsset.CreateTrack<ActionStateTrack>();
        }

        private static void AddHitColliderTrackGroup(TimelineAsset timelineAsset)
        {
            var group = timelineAsset.CreateTrack<GroupTrack>("Hit Collider Track Group");
            timelineAsset.CreateTrack<HitColliderTrack>(group, "Hit Collider");
        }

        private static void AddVfxTrackGroup(TimelineAsset timelineAsset)
        {
            var group = timelineAsset.CreateTrack<GroupTrack>("Vfx Track Group");
            timelineAsset.CreateTrack<ControlTrack>(group, "Vfx");
        }
    }
}