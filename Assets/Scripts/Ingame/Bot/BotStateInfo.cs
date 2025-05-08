using FMLib.Structs;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Bot
{
    public enum BotStateInfoType
    {
        Wait,
        MoveDirection,
        MoveToTarget,
        WalkAround,
        Action,
    }

    public struct BotStateInfo
    {
        public BotStateInfoType StateType;
        public VectorXZ Direction;
        public IEntity Target;
        public ActionData ActionData;
        public bool AttackNow;

        public static BotStateInfo CreateWait()
        {
            BotStateInfo info;
            info.StateType = BotStateInfoType.Wait;
            info.Direction = VectorXZ.Zero;
            info.Target = null;
            info.ActionData = null;
            info.AttackNow = false;
            return info;
        }

        public static BotStateInfo CreateMoveDirection(VectorXZ vec)
        {
            BotStateInfo info;
            info.StateType = BotStateInfoType.MoveDirection;
            info.Direction = vec;
            info.Target = null;
            info.ActionData = null;
            info.AttackNow = false;
            return info;
        }

        public static BotStateInfo CreateMoveToTarget(IEntity target)
        {
            BotStateInfo info;
            info.StateType = BotStateInfoType.MoveToTarget;
            info.Direction = VectorXZ.Zero;
            info.Target = target;
            info.ActionData = null;
            info.AttackNow = false;
            return info;
        }

        public static BotStateInfo CreateWalkAround(IEntity target)
        {
            BotStateInfo info;
            info.StateType = BotStateInfoType.WalkAround;
            info.Direction = VectorXZ.Zero;
            info.Target = target;
            info.ActionData = null;
            info.AttackNow = false;
            return info;
        }

        public static BotStateInfo CreateAttack(IEntity target, ActionData actionData, bool attackNow)
        {
            BotStateInfo info;
            info.StateType = BotStateInfoType.Action;
            info.Direction = VectorXZ.Zero;
            info.Target = target;
            info.ActionData = actionData;
            info.AttackNow = attackNow;
            return info;
        }
    }
}