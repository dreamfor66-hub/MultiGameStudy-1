using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Rogue.Tool.Timeline.Playables.TransformMove
{
    public class TransformMoveMixerBehaviour : PlayableBehaviour
    {
        public MoveDirection Direction;
        public float StartPos;
        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var trackBinding = playerData as Transform;

            if (!trackBinding)
                return;

            var inputCount = playable.GetInputCount();
            var totalPosition = StartPos;
            var globalTime = playable.GetTime();

            for (var i = 0; i < inputCount; i++)
            {
                var inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<TransformMoveBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();

                var localTime = inputPlayable.GetTime();
                var duration = input.EndTime - input.StartTime;
                if (globalTime <= input.StartTime)
                    localTime = 0f;
                else if (globalTime >= input.EndTime)
                    localTime = input.EndTime - input.StartTime;

                var move = input.LocalDistance((float)localTime, (float)duration);
                totalPosition += move;
                // Use the above variables to process each frame of this playable.
            }

            switch (Direction)
            {
                case MoveDirection.XDir:
                    {
                        var pos = trackBinding.position;
                        pos.x = totalPosition;
                        trackBinding.position = pos;
                        break;
                    }
                case MoveDirection.YDir:
                    {
                        var pos = trackBinding.position;
                        pos.y = totalPosition;
                        trackBinding.position = pos;
                        break;
                    }
                case MoveDirection.ZDir:
                    {
                        var pos = trackBinding.position;
                        pos.z = totalPosition;
                        trackBinding.position = pos;
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
