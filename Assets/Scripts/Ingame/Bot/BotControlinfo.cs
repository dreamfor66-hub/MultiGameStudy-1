using FMLib.Structs;
using Rogue.Ingame.Data;

namespace Rogue.Ingame.Bot
{
    public struct BotControlInfo
    {
        public VectorXZ Dir;
        public ActionData Action;

        public static BotControlInfo CreateWait()
        {
            BotControlInfo result;
            result.Dir = VectorXZ.Zero;
            result.Action = null;
            return result;
        }

        public static BotControlInfo CreateMove(VectorXZ vec)
        {
            BotControlInfo result;
            result.Dir = vec;
            result.Action = null;
            return result;
        }

        public static BotControlInfo CreateAction(VectorXZ vec, ActionData action)
        {
            BotControlInfo result;
            result.Dir = vec;
            result.Action = action;
            return result;
        }
    }
}