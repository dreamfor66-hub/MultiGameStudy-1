using Rogue.Ingame.Character;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveNone : IBuffActive
    {

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
        }
    }
}