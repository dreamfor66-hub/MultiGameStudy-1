using System;
using FMLib.Curve;
using Rogue.Ingame.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace Rogue.Tool.Timeline.Playables.CurvedAnimator
{
    [Serializable]
    public class CurvedAnimatorBehaviour : PlayableBehaviour
    {
        public AnimationClip Clip;
        public AnimationCurve Curve;

        [Button]
        public void ResetCurve()
        {
            if (Clip == null)
                return;
            Curve = CurveGenerator.Identity(Clip.length * CommonVariables.GameFrame);
        }

    }
}
