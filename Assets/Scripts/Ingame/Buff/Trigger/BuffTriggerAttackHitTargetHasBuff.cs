using System.Linq;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerAttackHitTargetHasBuff : BuffTriggerBase
    {
        private readonly ActionTypeMask typeMask;
        private readonly BuffTag buffTag;

        public BuffTriggerAttackHitTargetHasBuff(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
            typeMask = triggerData.AttackType;
            buffTag = triggerData.Tag;
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

        private void OnAttackHit(HitTotalInfo hitInfo)
        {
            if (hitInfo.Main.Attacker != Me && hitInfo.Main.AttackerRoot != Me)
                return;

            if (!ActionTypeHelper.CheckType(typeMask, hitInfo.Detail.ActionType))
                return;

            if (!hitInfo.Detail.IsDirect && !TriggerData.TriggerOnIndirectHit)
                return;

            var victimCharacter = hitInfo.Main.Victim.GameObject.GetComponent<CharacterBehaviour>();
            if (victimCharacter == null)
                return;

            var buffs = victimCharacter.BuffAccepter.GetBuffs();
            if (buffs.Any(x => x.Data.Tags.Any(t => t == buffTag)))
            {
                Invoke(hitInfo.Main.Victim);
            }
        }

    }
}