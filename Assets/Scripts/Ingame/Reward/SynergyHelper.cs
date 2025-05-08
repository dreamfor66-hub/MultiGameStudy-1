
namespace Rogue.Ingame.Reward
{
    public static class SynergyHelper
    {
        public static string GetString(SynergyReward reward)
        {
            return $"{reward.NeedCount} : {reward.Description}";
        }

        public static int GetTriggerLevel(SynergyData synergy, int count)
        {
            var idx = -1;
            for (var i = 0; i < synergy.Rewards.Count; i++)
            {
                if (count >= synergy.Rewards[i].NeedCount)
                    idx = i;
                else
                    break;
            }

            return idx;
        }
    }
}