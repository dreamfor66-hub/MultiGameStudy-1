using Rogue.Ingame.Data;

namespace Rogue.Ingame.Attack
{
    public struct HitstopInfo
    {
        public int HitFxId;
        public int ReductionIdx;
        public int AdditionalFrame;

        public bool IsValid()
        {
            return HitFxTable.Instance.IsValid(HitFxId) || ReductionIdx > 0 || AdditionalFrame > 0;
        }

        public static HitstopInfo CreateParrying(int hitFxId)
        {
            HitstopInfo info;
            info.HitFxId = hitFxId;
            info.ReductionIdx = 0;
            info.AdditionalFrame = CommonVariables.ParryingHitstopAddFrame;
            return info;
        }

        public static HitstopInfo CreateAttacker(int hitFxId, int reductionIdx)
        {
            HitstopInfo info;
            info.HitFxId = hitFxId;
            info.ReductionIdx = reductionIdx;
            info.AdditionalFrame = 0;
            return info;
        }

        public static HitstopInfo CreateVictim(int hitFxId)
        {
            HitstopInfo info;
            info.HitFxId = hitFxId;
            info.ReductionIdx = 0;
            info.AdditionalFrame = 0;
            return info;
        }
    }
}