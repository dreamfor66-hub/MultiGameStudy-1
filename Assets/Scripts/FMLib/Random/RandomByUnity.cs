namespace FMLib.Random
{
    public class RandomByUnity : IRandom
    {
        public static RandomByUnity Instance = new RandomByUnity();
        public int Range(int minInclusive, int maxExclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxExclusive);
        }

        public float Range(float minInclusive, float maxExclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxExclusive);
        }
    }
}