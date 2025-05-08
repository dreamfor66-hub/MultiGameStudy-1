using System;
using Rogue.Ingame.Data;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace Rogue.Tool.Timeline.Playables.TransformMove
{
    [Serializable]
    public class TransformMoveBehaviour : PlayableBehaviour
    {
        [HideLabel]
        public CurvePresetData Data;

        public AnimationCurve CalculateCurve(float duration)
        {
            return CurveCreator.Create(Data, duration);
        }

        public float LocalDistance(float localTime, float duration)
        {
            return CurveCreator.Create(Data, duration).Evaluate(localTime);
        }

        public double StartTime { get; set; }
        public double EndTime { get; set; }

        public override void OnPlayableCreate(Playable playable)
        {

        }
    }
}
