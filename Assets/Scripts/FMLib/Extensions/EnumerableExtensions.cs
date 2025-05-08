using System;
using System.Collections.Generic;

namespace FMLib.Extensions
{
    public static class EnumerableExtensions
    {
        public static T MinBy<T>(this IEnumerable<T> enumerable, Func<T, float> func)
        {
            var min = float.MaxValue;
            var ret = default(T);
            foreach (var t in enumerable)
            {
                var value = func(t);
                if (!(value < min)) continue;
                min = value;
                ret = t;
            }

            return ret;
        }

    }
}