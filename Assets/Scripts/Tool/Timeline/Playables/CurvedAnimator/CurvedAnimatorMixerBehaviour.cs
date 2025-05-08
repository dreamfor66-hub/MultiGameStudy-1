using Rogue.Ingame.Data;
using UnityEngine;
using UnityEngine.Playables;


namespace Rogue.Tool.Timeline.Playables.CurvedAnimator
{
    public class CurvedAnimatorMixerBehaviour : PlayableBehaviour
    {
        private static float GameFrame => CommonVariables.GameFrame;

        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var animator = playerData as Animator;
            if (animator == null)
                return;

            var inputCount = playable.GetInputCount();
            animator.speed = 0f;
            for (var i = 0; i < inputCount; i++)
            {
                var inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<CurvedAnimatorBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();

                if (inputWeight < 0.5f)
                    continue;

                var localTime = inputPlayable.GetTime();
                var convertedTime = input.Curve.Evaluate((float)localTime * GameFrame) / GameFrame;
                Play(animator, input.Clip, convertedTime);
            }
        }

        private static void Play(Animator animator, AnimationClip clip, float time)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                UnityEditor.AnimationMode.SampleAnimationClip(animator.gameObject, clip, time);
            }
            else
            {
                UnityEditor.AnimationMode.BeginSampling();
                UnityEditor.AnimationMode.SampleAnimationClip(animator.gameObject, clip, time);
                UnityEditor.AnimationMode.EndSampling();
            }
#endif
        }
    }
}
