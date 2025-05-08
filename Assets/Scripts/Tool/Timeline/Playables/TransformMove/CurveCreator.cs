using System;
using UnityEngine;

namespace Rogue.Tool.Timeline.Playables.TransformMove
{
    public static class CurveCreator
    {
        public static AnimationCurve Create(CurvePresetData data, float duration)
        {
            switch (data.Type)
            {
                case MoveCurveType.Stop:
                    return new CurveCreatorStop().Create(duration);
                case MoveCurveType.ConstantVelocityEaseInOut:
                    return new CurveCreatorConstantVelocityEaseInOut(data).Create(duration);
                case MoveCurveType.FixedDistanceLinear:
                    return new CurveCreatorFixedDistanceLinear(data).Create(duration);
                case MoveCurveType.FixedDistanceStartEndVel:
                    return new CurveCreatorFixedDistanceStartEndVel(data).Create(duration);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}