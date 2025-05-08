using System.Collections.Generic;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Character;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerCollideDuringAction : BuffTriggerBase
    {
        private readonly ActionTypeMask typeMask;
        private readonly CharacterBehaviour character;
        private bool isOn = false;
        private Vector3 prevPosition;

        public BuffTriggerCollideDuringAction(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
            typeMask = triggerData.AttackType;
            character = me.GameObject.GetComponent<CharacterBehaviour>();
        }

        protected override void OnStart()
        {
        }

        private readonly RaycastHit[] hits = new RaycastHit[8];
        private HashSet<IEntity> prevHit = new HashSet<IEntity>();

        protected override void OnUpdate()
        {
            if (character == null)
                return;
            var nowOn = false;
            var stateInfo = character.TotalInfo.StateInfo;
            if (stateInfo.StateType == CharacterStateType.Action)
            {
                var actionData = stateInfo.ActionData;
                if (ActionTypeHelper.CheckType(typeMask, actionData.AttackType))
                    nowOn = true;
            }

            if (!isOn && nowOn)
                TurnOn();
            else if (isOn && !nowOn)
                TurnOff();

            if (isOn)
            {
                var curPosition = character.transform.position;
                var radius = 0.4f;
                var offset = new Vector3(0f, 0.2f, 0f);
                var point1 = prevPosition + offset;
                var point2 = curPosition + offset;
                var hitCount = Physics.CapsuleCastNonAlloc(point1, point2, radius, Vector3.up, hits, 0f, LayerHelper.HurtboxMask);

                for (var i = 0; i < hitCount; i++)
                {
                    var hit = hits[i];
                    var hurtbox = hit.collider.GetComponent<Hurtbox>();
                    var entity = hurtbox.Entity;
                    if (prevHit.Contains(entity))
                        continue;
                    prevHit.Add(entity);
                    if (entity == Me)
                        continue;
                    Invoke(entity);
                }
                prevPosition = curPosition;
            }
        }

        protected override void OnEnd()
        {
        }

        private void TurnOn()
        {
            isOn = true;
            prevHit.Clear();
            prevPosition = character.transform.position;
        }

        private void TurnOff()
        {
            isOn = false;
        }
    }
}