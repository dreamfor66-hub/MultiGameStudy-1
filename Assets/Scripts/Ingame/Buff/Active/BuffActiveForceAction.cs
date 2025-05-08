using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using UnityEngine;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveForceAction : IBuffActive
    {
        private readonly ActionData action;
        public BuffActiveForceAction(BuffActiveData data)
        {
            action = data.Action;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            GameCommandForceAction.Send(target, action);
        }
    }
}