using System.Collections.Generic;
using System.Linq;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff
{
    public class BuffUnit
    {
        private readonly BuffUnitData buffUnitData;
        private readonly BuffCondition buffCondition;
        private readonly BuffValues buffValues;
        private readonly BuffInstance buffInstance;
        private readonly IEntity me;
        private readonly IEntity root;
        private readonly List<BuffEffectSimpleValue> simpleValues;
        private readonly List<BuffEffectTriggerActive> triggerActives;
        private readonly List<BuffEffectOverrideHit> overrideHits;
        private bool isOn;

        public BuffUnit(BuffUnitData buffUnitData, BuffCondition buffCondition, BuffValues buffValues, BuffInstance buffInstance, IEntity me, IEntity root)
        {
            this.buffUnitData = buffUnitData;
            this.buffCondition = buffCondition;
            this.buffValues = buffValues;
            this.buffInstance = buffInstance;
            this.me = me;
            this.root = root;
            simpleValues = buffUnitData.SimpleEffects.Select(CreateSimpleValue).ToList();
            triggerActives = buffUnitData.TriggerActiveEffects.Select(CreateTriggerActive).ToList();
            overrideHits = buffUnitData.OverrideHitEffects.Select(CreateOverrideHit).ToList();
            isOn = false;
        }

        public void OnStart()
        {
            CheckCondition();
        }

        public void OnUpdate()
        {
            if (isOn)
            {
                foreach (var simple in simpleValues)
                {
                    simple.OnUpdate();
                }
                foreach (var ta in triggerActives)
                {
                    ta.OnUpdate();
                }
            }
            CheckCondition();
        }

        public void OnEnd()
        {
            if (isOn)
                TurnOff();
        }

        public void OnStackCountChanged(int cur)
        {
            foreach (var simple in simpleValues)
            {
                simple.OnStackCountChanged(cur);
            }
        }

        private void CheckCondition()
        {
            var cur = IsConditionSatisfied();
            var prev = isOn;

            if (cur && !prev)
                TurnOn();
            else if (!cur && prev)
                TurnOff();
        }

        private void TurnOn()
        {
            foreach (var simple in simpleValues)
            {
                simple.OnStart();
            }

            foreach (var ta in triggerActives)
            {
                ta.OnStart();
            }
            isOn = true;
        }

        private void TurnOff()
        {
            foreach (var simple in simpleValues)
            {
                simple.OnEnd();
            }

            foreach (var ta in triggerActives)
            {
                ta.OnEnd();
            }
            isOn = false;
        }

        public void OverrideHit(HitInfo hitInfo, ref HitBuffInfo buffInfo)
        {
            if (!isOn)
                return;
            foreach (var overrider in overrideHits)
            {
                overrider.OverrideHit(hitInfo, ref buffInfo);
            }
        }

        public void OverrideHurt(HitInfo hitInfo, ref HitBuffInfo buffInfo)
        {
            if (!isOn)
                return;

            foreach (var overrider in overrideHits)
            {
                overrider.OverrideHurt(hitInfo, ref buffInfo);
            }
        }


        private bool IsConditionSatisfied()
        {
            return buffUnitData.Conditions.All(x => buffCondition.IsConditionSatisfied(x, root));
        }

        private BuffEffectSimpleValue CreateSimpleValue(BuffEffectSimpleData data)
        {
            return new BuffEffectSimpleValue(data, buffCondition, buffValues);
        }

        private BuffEffectTriggerActive CreateTriggerActive(BuffEffectTriggerActiveData data)
        {
            return new BuffEffectTriggerActive(data, me, root, buffInstance);
        }

        private BuffEffectOverrideHit CreateOverrideHit(BuffEffectOverrideHitData data)
        {
            return new BuffEffectOverrideHit(data, buffInstance);
        }

    }
}