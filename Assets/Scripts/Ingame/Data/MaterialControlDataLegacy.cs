using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    public enum MatcapType
    {
        None,
        Additive,
        Multiplicative,
        MatcapOnly
    }




    public class MaterialControlDataLegacy : ScriptableObject
    {
        public bool EnableMatcap;
        [ShowIf("EnableMatcap")]
        public MatcapType MatcapType = MatcapType.None;
        [ShowIf("EnableMatcap")]
        public Texture MatcapTexture;
        [ShowIf("EnableMatcap")]
        public AnimationCurve MatcapCurve;

        public DurationType DurationType;
        public float Duration;

        [Tooltip("수치가 높을수록 우선순위가 높음")]
        public int Priority;
    }
}