using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Bot
{
    public static class BotHelper
    {
        public static int GetWaitFrame(this BotPhaseActionLegacy phaseActionLegacy)
        {
            return Random.Range(phaseActionLegacy.WaitFrameMin, phaseActionLegacy.WaitFrameMax + 1);
        }
    }
}