using System.Linq;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;
using UnityEngine;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveReviveRandomMemberHpPercent : IBuffActive
    {
        private readonly int percent;

        public BuffActiveReviveRandomMemberHpPercent(BuffActiveData data)
        {
            percent = data.Value;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            var team = target.Team;
            var deadChars = EntityTable.Entities.Where(x => x.Team == team && IsDead(x)).ToArray();

            if (deadChars.Length == 0)
                return;
            var selected = deadChars[Random.Range(0, deadChars.Length)];
            GameCommandRevive.Send(selected, percent);
        }

        public bool IsDead(IEntity entity)
        {
            var character = entity.GameObject.GetComponent<CharacterBehaviour>();
            if (character == null)
                return false;
            return character.IsDead;
        }
    }
}