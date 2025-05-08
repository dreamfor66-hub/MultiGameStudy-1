using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rogue.Ingame.Network
{
    public class FrameDeJitter
    {
        public int Frame { get; private set; }
        private readonly List<int> recentJitter = new List<int>();

        private const int ResetDiff = 10;
        private const int BufferSize = 30;
        private const int MaxMinusCount = 2;

        public void Set(int frame)
        {
            if (Mathf.Abs(Frame - frame) > ResetDiff)
            {
                Frame = frame;
                recentJitter.Clear();
                recentJitter.Add(0);
            }
            else
            {
                recentJitter.Add(frame - Frame);
            }

            while (recentJitter.Count > BufferSize)
            {
                recentJitter.RemoveAt(0);
            }

            var change = GetChange();
            if (change != 0)
            {
                Frame += change;
                for (int i = 0; i < recentJitter.Count; i++)
                {
                    recentJitter[i] -= change;
                }
            }
        }

        private int GetChange()
        {
            var minusCount = recentJitter.Count(x => x < 0);
            if (minusCount > MaxMinusCount)
                return -1;
            var avg = (float)recentJitter.Sum() / recentJitter.Count;

            if (avg < 0f)
                return -1;

            var zeroMinusCount = recentJitter.Count(x => x <= 0);

            if (avg > 1f && zeroMinusCount <= MaxMinusCount)
                return 1;

            return 0;
        }

        public void UpdateFrame()
        {
            Frame++;
        }
    }
}