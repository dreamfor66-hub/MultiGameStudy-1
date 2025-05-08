using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerCumulativeDamageMoreThanHpPercent : BuffTriggerBase
    {
        private readonly int percent;
        private int damageSum = 0;
        private readonly HpModule hpModule;
        public BuffTriggerCumulativeDamageMoreThanHpPercent(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
            percent = triggerData.Value;
            hpModule = me.GameObject.GetComponent<CharacterBehaviour>().HpModule;
        }

        protected override void OnStart()
        {
            BuffTriggerDispatcher.OnAttackHit += OnAttackHit;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnEnd()
        {
            BuffTriggerDispatcher.OnAttackHit -= OnAttackHit;
        }

        private void OnAttackHit(HitTotalInfo hit)
        {
            if (hit.Main.Victim != Me)
                return;

            damageSum += hit.Damage.FinalDamage;

            var damageCut = hpModule.HpInfo.MaxHp * percent / 100f;

            if (damageSum >= damageCut)
            {
                Invoke(hit.Main.Victim);
                damageSum -= (int)damageCut;
            }
        }
    }
}