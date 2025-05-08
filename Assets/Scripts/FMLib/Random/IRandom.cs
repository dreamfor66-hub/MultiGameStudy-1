namespace FMLib.Random
{
    public interface IRandom
    {
        int Range(int minInclusive, int maxExclusive);
        float Range(float minInclusive, float maxExclusive);
    }
}