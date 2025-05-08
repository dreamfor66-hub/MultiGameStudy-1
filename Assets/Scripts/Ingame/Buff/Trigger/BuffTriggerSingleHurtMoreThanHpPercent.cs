using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using HpModule = Rogue.Ingame.Character.HpModule;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerSingleHurtMoreThanHpPercent : BuffTriggerBase
    {
        private readonly int percent;
        private readonly HpModule hpModule;

        public BuffTriggerSingleHurtMoreThanHpPercent(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
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

            var damageCut = hpModule.HpInfo.MaxHp * percent / 100f;
            if (hit.Damage.FinalDamage >= damageCut)
                Invoke(hit.Main.Victim);
        }
    }
}