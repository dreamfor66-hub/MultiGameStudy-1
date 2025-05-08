using Rogue.Ingame.Attack;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerAttackHit : BuffTriggerBase
    {
        private readonly ActionTypeMask typeMask;
        public BuffTriggerAttackHit(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
            typeMask = triggerData.AttackType;
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

            if (!CheckEntity(hitInfo.Main.Victim, TriggerData.EntityType))
                return;

            if (!hitInfo.Detail.IsDirect && !TriggerData.TriggerOnIndirectHit)
                return;

            Invoke(hitInfo.Main.Victim);
        }

        //Derived from BuffEffectTriggerActive.CheckEntity()
        private bool CheckEntity(IEntity target, BuffTargetEntityType entityType)
        {
            if (target is CharacterBehaviour character)
                if (character.IsDead)
                    return false;

            if (entityType == BuffTargetEntityType.All)
                return true;

            if ((entityType & BuffTargetEntityType.Character) != 0)
                if (target is CharacterBehaviour)
                    return true;

            if ((entityType & BuffTargetEntityType.Bullet) != 0)
                if (target is BulletBehaviour)
                    return true;

            return false;
        }
    }
}