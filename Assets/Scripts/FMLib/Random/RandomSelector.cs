using System;
using System.Collections.Generic;
using System.Linq;

namespace FMLib.Random
{
    public static class RandomSelector
    {
        public static int Select(IRandom random, IEnumerable<int> probTable)
        {
            var sum = probTable.Sum();
            if (sum <= 0)
                return -1;
            var selected = random.Range(0, sum);

            var curIdx = 0;
            var curSum = 0;
            foreach (var prob in probTable)
            {
                curSum += prob;
                if (selected < curSum)
                    return curIdx;
                curIdx++;
            }

            throw new Exception("Random Selector Unknown Error");
        }

        public static int Select(IRandom random, IEnumerable<float> probTable)
        {
            var sum = probTable.Sum();
            if (sum <= 0)
                return -1;
            var selected = random.Range(0, sum);

            var curIdx = 0;
            var curSum = 0f;
            foreach (var prob in probTable)
            {
                curSum += prob;
                if (selected < curSum)
                    return curIdx;
                curIdx++;
            }

            throw new Exception("Random Selector Unknown Error");
        }

        public static IEnumerable<T> SelectN<T>(this IEnumerable<T> enumerable, IRandom random, int count)
        {
            var remain = enumerable.Count();
            var remainSelect = count;
            foreach (var t in enumerable)
            {
                if (random.Range(0, remain) < remainSelect)
                {
                    yield return t;
                    remainSelect -= 1;
                }
                remain -= 1;
            }
        }
    }
}