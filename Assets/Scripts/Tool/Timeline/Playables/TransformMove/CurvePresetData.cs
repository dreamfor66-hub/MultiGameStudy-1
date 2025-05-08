using System;
using Sirenix.OdinInspector;

namespace Rogue.Tool.Timeline.Playables.TransformMove
{
    public enum MoveCurveType
    {
        Stop,

        ConstantVelocityEaseInOut = 2,

        FixedDistanceLinear = 10,
        FixedDistanceStartEndVel,
    }

    [Serializable]
    public class CurvePresetData
    {
        [EnumToggleButtons, HideLabel]
        public MoveCurveType Type;

        private bool ShowDistance =>
            Type == MoveCurveType.FixedDistanceLinear || Type == MoveCurveType.FixedDistanceStartEndVel;

        private bool ShowStartEndVel =>
            Type == MoveCurveType.ConstantVelocityEaseInOut || Type == MoveCurveType.FixedDistanceStartEndVel;

        [ShowIf(nameof(ShowDistance))]
        public float Distance;

        [ShowIf(nameof(Type), MoveCurveType.ConstantVelocityEaseInOut)]
        public float Vel;

        [ShowIf(nameof(Type), MoveCurveType.ConstantVelocityEaseInOut)]
        public float EaseInFrame;

        [ShowIf(nameof(ShowStartEndVel))]
        public float StartVel;

        [ShowIf(nameof(Type), MoveCurveType.ConstantVelocityEaseInOut)]
        public float EaseOutFrame;

        [ShowIf(nameof(ShowStartEndVel))]
        public float EndVel;
    }
}