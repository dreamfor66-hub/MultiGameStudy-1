using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Rogue.Ingame.Data;

namespace Rogue.Tool.Timeline.Playables.ActionState
{
    public enum ActionStateKeyPreset
    {
        Evadable = 1,
        SkillAvailable,

        LightCombo01 = 10,
        LightCombo02,
        LightCombo03,

        BotRepeatExitable,
        BotChainExitable,

        Custom = 100,
    }


    public class ActionStateCustomClip : PlayableAsset, ITimelineClipAsset
    {
        [OnValueChanged(nameof(OnKeyChanged))]
        public string Key;

        [OnValueChanged(nameof(OnPresetChanged))]
        public ActionStateKeyPreset Preset;

        public override double duration
        {
            get
            {
                return CommonVariables.TimelineDefaultClipLength;
            }
        }

        private void OnKeyChanged()
        {
            ChangeDisplayName();
        }

        private void ChangeDisplayName()
        {
            var director = FindObjectOfType<PlayableDirector>();
            var timeline = director.playableAsset as TimelineAsset;
            var tracks = timeline.GetOutputTracks().OfType<ActionStateTrack>();

            foreach (var track in tracks)
            {
                foreach (var clip in track.GetClips())
                {
                    if (clip.asset == this)
                    {
                        clip.displayName = Key;
                    }
                }
            }
        }

        private void OnPresetChanged(ActionStateKeyPreset preset)
        {
            if (preset != ActionStateKeyPreset.Custom)
            {
                Key = preset.ToString();
                ChangeDisplayName();
            }
        }


        public ClipCaps clipCaps => ClipCaps.None;



        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return new Playable();
        }
    }
}