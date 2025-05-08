using Rogue.Ingame.Vfx;
using UnityEngine;

namespace Rogue.Ingame.Data
{
    [CreateAssetMenu(fileName = "new HitFxData", menuName = "Data/Hit Fx")]
    public class HitFxData : ScriptableObject
    {
        public AnimationCurve HitstopCurve;
        public float HitstopTime;

        public float CameraShakeLength;
        public float VibrationTime;

        public AudioClip HitClip;
        public VfxObject HitVfx;


        public float GetHitstopSpeed(float frame)
        {
            if (HitstopTime == 0f)
            {
                return 0f;
            }
            else
            {
                var timeInCurve = frame / CommonVariables.GameFrame / HitstopTime;
                return HitstopCurve.Evaluate(timeInCurve);
            }
        }

        public int GetHitstopMaxFrame()
        {
            var time = HitstopCurve.keys[HitstopCurve.keys.Length - 1].time * HitstopTime;
            var frame = (int)(time * CommonVariables.GameFrame);
            return frame;
        }
    }
}