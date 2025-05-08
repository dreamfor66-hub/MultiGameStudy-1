namespace FMLib.Random
{
    public class RandomWithSeed : IRandom
    {
        private readonly System.Random random;
        public RandomWithSeed(int seed)
        {
            random = new System.Random(seed);
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            return random.Next(minInclusive, maxExclusive);
        }

        public float Range(float minInclusive, float maxExclusive)
        {
            return (float)(random.NextDouble() * (maxExclusive - minInclusive) + minInclusive);
        }

    }
}