using System;
using System.Collections.Generic;
using System.Linq;
using FMLib.Curve;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Vfx;
using Rogue.Tool.Timeline.Playables.ActionState;
using Rogue.Tool.Timeline.Playables.CharacterStatus;
using Rogue.Tool.Timeline.Playables.CurvedAnimator;
using Rogue.Tool.Timeline.Playables.HitCollider;
using Rogue.Tool.Timeline.Playables.TransformMove;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Rogue.Tool.Timeline.ToAction
{
    public class TimelineToActionConverter
    {
        private readonly TimelineToActionInfo info;

        public TimelineToActionConverter(TimelineToActionInfo info)
        {
            this.info = info;
        }

        public void Convert()
        {
            if (!CheckValidInfo(info))
            {
                Debug.LogError("Invalid info");
                return;
            }

            ConvertCommonAndAnimation();
            ConvertMove();
            ConvertAttack();
            ConvertState();
            ConvertVfx();
            ConvertStatus();
            SaveAssets();
        }

        private static bool CheckValidInfo(TimelineToActionInfo info)
        {
            if (info.PlayableDirector == null) return false;
            if (string.IsNullOrEmpty(info.AnimationKey)) return false;
            if (info.ActionData == null) return false;
            if (info.Controller == null) return false;
            return true;
        }

        private TimelineClip GetAnimationTimelineClip()
        {
            var timeline = info.PlayableDirector.playableAsset as TimelineAsset;
            var animTrack = timeline.GetOutputTracks().First(x => x is CurvedAnimatorTrack);
            return animTrack.GetClips().First();
        }

        private AnimationClip GetAnimationClip()
        {
            var animAsset = GetAnimationTimelineClip().asset as CurvedAnimatorClip;
            return animAsset.template.Clip;
        }

        private AnimationCurve GetAdjustCurve()
        {
            var animAsset = GetAnimationTimelineClip().asset as CurvedAnimatorClip;
            return animAsset.template.Curve;
        }

        private void ConvertCommonAndAnimation()
        {
            var animClip = GetAnimationClip();
            var adjustCurve = GetAdjustCurve();
            var originalFrame = Mathf.RoundToInt(animClip.length * CommonVariables.GameFrame);
            var totalFrame = Mathf.RoundToInt(adjustCurve.keys.Last().time);

            info.ActionData.ActionKey = info.AnimationKey;
            info.ActionData.AnimationOriginalFrame = originalFrame;
            info.ActionData.TotalFrame = totalFrame;
            info.ActionData.AnimationAdjustCurve = adjustCurve;
            AddAnimationState(info.AnimationKey, animClip);
        }

        private void AddAnimationState(string name, AnimationClip clip)
        {
            var stateMachine = info.Controller.layers[0].stateMachine;
            var prev = stateMachine.states.Select(x => x.state).FirstOrDefault(x => x.name == name);
            if (prev != null)
            {
                prev.motion = clip;
            }
            else
            {
                var state = stateMachine.AddState(name);
                state.motion = clip;
            }
        }

        private void ConvertMove()
        {
            var timeline = info.PlayableDirector.playableAsset as TimelineAsset;
            var moveTracks = timeline.GetOutputTracks().OfType<TransformMoveTrack>();
            var converter = new TimelineMoveConverter();
            info.ActionData.MoveCurve = new AnimationCurve();
            info.ActionData.MoveCurveX = new AnimationCurve();
            info.ActionData.MoveCurveY = new AnimationCurve();

            foreach (var track in moveTracks)
            {
                switch (track.MoveDirection)
                {
                    case MoveDirection.XDir:
                        info.ActionData.MoveCurveX = converter.GetMoveCurve(track);
                        break;
                    case MoveDirection.YDir:
                        info.ActionData.MoveCurveY = converter.GetMoveCurve(track);
                        break;
                    case MoveDirection.ZDir:
                        info.ActionData.MoveCurve = converter.GetMoveCurve(track);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ConvertAttack()
        {
            var timeline = info.PlayableDirector.playableAsset as TimelineAsset;
            var hitColliderTracks = timeline.GetOutputTracks().OfType<HitColliderTrack>();
            info.ActionData.AttackHitboxData = new List<AttackHitboxData>();
            foreach (var track in hitColliderTracks)
            {
                var anchor = info.PlayableDirector.GetGenericBinding(track) as HitColliderAnchor;
                if (anchor == null)
                    continue;
                var id = anchor.Id;
                var clips = track.GetClips();

                foreach (var clip in clips)
                {
                    var colliderClip = clip.asset as HitColliderClip;

                    var hitbox = new AttackHitboxData();
                    hitbox.ColliderId = id;
                    hitbox.StartFrame = Mathf.RoundToInt((float)clip.start * CommonVariables.GameFrame);
                    hitbox.EndFrame = Mathf.RoundToInt((float)clip.end * CommonVariables.GameFrame);

                    hitbox.GroupId = colliderClip.GroupId;
                    hitbox.Info = colliderClip.Info;
                    hitbox.BuffData = colliderClip.BuffData;
                    info.ActionData.AttackHitboxData.Add(hitbox);
                }
            }
        }

        private void ConvertState()
        {
            var timeline = info.PlayableDirector.playableAsset as TimelineAsset;
            var stateTracks = timeline.GetOutputTracks().OfType<ActionStateTrack>();

            info.ActionData.CustomStates = new List<ActionStateData>();

            var lastFrame = info.ActionData.TotalFrame;
            var exitableFrame = lastFrame;

            foreach (var track in stateTracks)
            {
                foreach (var clip in track.GetClips())
                {
                    if (clip.asset is ActionStateExitableClip)
                    {
                        exitableFrame = GetFrame(clip.start);
                    }
                    else if (clip.asset is ActionStateCustomClip customClip)
                    {
                        info.ActionData.CustomStates.Add(CustomClipToData(clip, customClip));
                    }
                }
            }
            info.ActionData.ExitableFrame = exitableFrame;
        }

        private int GetFrame(double clipTime)
        {
            return Mathf.RoundToInt((float)clipTime * CommonVariables.GameFrame);
        }

        private void ConvertVfx()
        {
            var timeline = info.PlayableDirector.playableAsset as TimelineAsset;
            var controlTracks = timeline.GetOutputTracks().OfType<ControlTrack>();
            if (!info.PlayableDirector.playableGraph.IsValid())
                info.PlayableDirector.RebuildGraph();
            var graph = info.PlayableDirector.playableGraph;

            info.ActionData.VfxData.Clear();

            foreach (var track in controlTracks)
            {
                var clips = track.GetClips();
                foreach (var clip in clips)
                {
                    var asset = clip.asset as ControlPlayableAsset;
                    if (asset == null)
                        continue;

                    var gameObject = asset.sourceGameObject.Resolve(graph.GetResolver());
                    var prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                    if (prefab == null)
                    {
                        Debug.LogError($"can't find prefab : {gameObject.name}");
                        continue;
                    }

                    var vfxObject = prefab.GetComponent<VfxObject>();
                    if (vfxObject == null)
                    {
                        Debug.LogError($"{gameObject.name} 에 Vfx Object 컴포넌트가 필요합니다.(생성된 오브젝트가 아닌 프로젝트 내 프리팹에 붙여주세요)");
                        continue;
                    }

                    if (gameObject.transform.parent != null)
                    {
                        if (vfxObject.FollowType == FollowType.None)
                            Debug.LogError($"{gameObject.name} 은 프리팹 하위에 붙어있으므로 Vfx FollowType 이 None 이 아니어야 합니다.");

                        var frame = Mathf.RoundToInt((float)clip.start * CommonVariables.GameFrame);
                        var data = new ActionVfxData();
                        data.Prefab = vfxObject;
                        var scale = gameObject.transform.parent.localScale;
                        data.Position = Vector3.Scale(gameObject.transform.localPosition, scale);
                        data.Rotation = gameObject.transform.localRotation;
                        data.Scale = Vector3.Scale(gameObject.transform.localScale, scale);
                        data.StartFrame = frame;
                        info.ActionData.VfxData.Add(data);
                    }
                    else
                    {
                        if (vfxObject.FollowType != FollowType.None)
                            Debug.LogError($"{gameObject.name} 은 프리팹 하위에 붙어있지 않으므로 Vfx FollowType 이 None 이어야 합니다.");
                        var frame = Mathf.RoundToInt((float)clip.start * CommonVariables.GameFrame);
                        var charPosition = new Vector3(info.ActionData.MoveCurveX.Evaluate((float)clip.start), 0f, info.ActionData.MoveCurve.Evaluate((float)clip.start));
                        var data = new ActionVfxData();
                        data.Prefab = vfxObject;
                        data.Position = gameObject.transform.position - charPosition;
                        data.Rotation = gameObject.transform.rotation;
                        data.Scale = gameObject.transform.localScale;
                        data.StartFrame = frame;
                        info.ActionData.VfxData.Add(data);
                    }
                }
            }
        }

        private ActionStateData CustomClipToData(TimelineClip clip, ActionStateCustomClip customClip)
        {
            var state = new ActionStateData();
            state.Key = customClip.Key;
            state.StartFrame = GetFrame(clip.start);
            state.EndFrame = GetFrame(clip.end);
            return state;
        }


        private void ConvertStatus()
        {
            var timeline = info.PlayableDirector.playableAsset as TimelineAsset;
            var statusTracks = timeline.GetOutputTracks().OfType<CharacterStatusTrack>();

            info.ActionData.StatusData.Clear();
            foreach (var track in statusTracks)
            {
                var clips = track.GetClips();
                foreach (var clip in clips)
                {
                    CharacterStatusData data = new CharacterStatusData();
                    data.StartFrame = Mathf.RoundToInt((float)clip.start * CommonVariables.GameFrame);
                    data.EndFrame = Mathf.RoundToInt((float)clip.end * CommonVariables.GameFrame);
                    data.Type = clip.asset switch
                    {
                        CharacterStatusGhostClip ghostClip => CharacterStatusType.Ghost,
                        CharacterStatusSuperArmorClip superArmorClip => CharacterStatusType.SuperArmor,
                        CharacterStatusIntangibleClip intangibleClip => CharacterStatusType.Intangible,
                        CharacterStatusInvisibleClip invisibleClip => CharacterStatusType.Invisible,
                        CharacterStatusParryingClip parryingClip => CharacterStatusType.Parrying,
                        _ => throw new ArgumentException($"unknown type clip : {clip.asset.name}")
                    };
                    info.ActionData.StatusData.Add(data);
                }
            }
        }

        private void SaveAssets()
        {
            EditorUtility.SetDirty(info.ActionData);
            EditorUtility.SetDirty(info.Controller);
            AssetDatabase.SaveAssetIfDirty(info.ActionData);
            AssetDatabase.SaveAssetIfDirty(info.Controller);
        }
    }
}