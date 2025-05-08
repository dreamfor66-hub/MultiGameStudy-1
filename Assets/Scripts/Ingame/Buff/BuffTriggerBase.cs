using System;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rogue.Ingame.Buff
{
    public abstract class BuffTriggerBase
    {
        public Action<IEntity> OnTrigger;

        protected IEntity Me { get; private set; }
        protected BuffTriggerData TriggerData { get; private set; }

        private int repeatCount;
        private int stageCooldown;
        private float lastInvokeTime = 0f;

        protected BuffTriggerBase(IEntity me, BuffTriggerData triggerData)
        {
            Me = me;
            TriggerData = triggerData;
            repeatCount = 0;
            stageCooldown = 0;
        }

        protected abstract void OnStart();
        protected abstract void OnUpdate();
        protected abstract void OnEnd();

        public void Start()
        {
            if (TriggerData.CooldownStage > 0)
                BuffTriggerDispatcher.OnClearStage += ClearStage;
            OnStart();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void End()
        {
            if (TriggerData.CooldownStage > 0)
                BuffTriggerDispatcher.OnClearStage -= ClearStage;
            OnEnd();
        }

        private void ClearStage()
        {
            stageCooldown--;
        }

        protected void Invoke(IEntity target)
        {
            repeatCount++;
            if (repeatCount < TriggerData.EveryNTimes)
                return;

            repeatCount = 0;

            if (TriggerData.Chance < 100f)
            {
                float roll = Random.Range(0f, 100f);
                if (roll >= TriggerData.Chance)
                    return;
            }

            if (TriggerData.CooldownFrame > 0)
            {
                var minNextTime = lastInvokeTime + (float)TriggerData.CooldownFrame / CommonVariables.GameFrame;
                if (minNextTime > Time.time)
                    return;
            }

            if (TriggerData.CooldownStage > 0)
            {
                if (stageCooldown > 0)
                    return;
                stageCooldown = TriggerData.CooldownStage;
            }

            OnTrigger?.Invoke(target);
            lastInvokeTime = Time.time;
        }
    }
}