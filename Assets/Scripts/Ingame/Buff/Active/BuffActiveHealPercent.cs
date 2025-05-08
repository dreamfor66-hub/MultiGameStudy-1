using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Buff.Active
{
    public class BuffActiveHealPercent : IBuffActive
    {
        private readonly int percent;

        public BuffActiveHealPercent(BuffActiveData data)
        {
            percent = data.Value;
        }

        public void DoActive(IEntity me, IEntity target, IEntity rootSource, BuffInstance buff)
        {
            if (target is CharacterBehaviour character)
            {
                var amount = character.HpModule.HpInfo.MaxHp * percent / 100;
                GameCommandHeal.Send(new HealInfo(target, amount));
            }

        }
    }
}