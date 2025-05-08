using System;
using Rogue.Ingame.Reward;

namespace Rogue.Ingame.Stage
{
    public static class RewardHelper
    {
        public static string TagToString(SynergyTag tag)
        {
            switch (tag)
            {
                case SynergyTag.Fire:
                    return "화염";
                case SynergyTag.Ice:
                    return "빙결";
                case SynergyTag.Spear:
                    return "죽창";
                case SynergyTag.Heal:
                    return "회복";
                case SynergyTag.Wind:
                    return "바람";
                case SynergyTag.Drain:
                    return "흡혈";
                case SynergyTag.Plague:
                    return "역병";
                case SynergyTag.Spine:
                    return "가시";
                default:
                    throw new ArgumentOutOfRangeException(nameof(tag), tag, null);
            }
        }
    }
}