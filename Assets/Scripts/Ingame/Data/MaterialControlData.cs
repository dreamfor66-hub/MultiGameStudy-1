using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    [Serializable]
    [Toggle("Enabled")]
    public class MaterialEmissionData
    {
        public bool Enabled;
        public Color Color;
        public AnimationCurve Curve;
    }

    [Serializable]
    [Toggle("Enabled")]
    public class MaterialFresnelData
    {
        public bool Enabled;
        public Color Color;
        public AnimationCurve ThresholdCurve;
        public AnimationCurve SmoothCurve;
    }

    public enum DurationType
    {
        FixedDuration,
        Manual
    }

    [CreateAssetMenu(fileName = "new MaterialControlData", menuName = "Data/Material Control")]
    public class MaterialControlData : ScriptableObject
    {
        public DurationType DurationType;
        public float Duration;
        [Tooltip("수치가 높을수록 우선순위가 높음")]
        public int Priority;

        public MaterialEmissionData Emission;
        public MaterialFresnelData Fresnel;
    }
}