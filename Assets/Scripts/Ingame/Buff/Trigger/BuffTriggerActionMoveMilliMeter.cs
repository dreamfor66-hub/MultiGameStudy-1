using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerActionMoveMilliMeter : BuffTriggerBase
    {
        private readonly ActionTypeMask typeMask;
        private readonly float periodDist;
        private readonly CharacterBehaviour character;

        private Vector3 prevPosition;
        private bool isOn;
        private float leftDistance;

        public BuffTriggerActionMoveMilliMeter(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
            typeMask = triggerData.AttackType;
            periodDist = triggerData.Value / 1000f;
            character = me.GameObject.GetComponent<CharacterBehaviour>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnUpdate()
        {
            if (character == null)
                return;
            if (periodDist <= 0f)
                return;

            var nowOn = false;
            var stateInfo = character.TotalInfo.StateInfo;
            if (stateInfo.StateType == CharacterStateType.Action)
            {
                var actionData = stateInfo.ActionData;
                if (ActionTypeHelper.CheckType(typeMask, actionData.AttackType))
                {
                    nowOn = true;
                }
            }

            if (!isOn && nowOn)
                TurnOn();
            else if (isOn && !nowOn)
                TurnOff();

            if (isOn)
            {
                var curPosition = character.transform.position;
                var dir = (curPosition - prevPosition).normalized;
                var curDist = (curPosition - prevPosition).magnitude;
                var moveDist = (periodDist - leftDistance);
                var beforePosition = prevPosition;

                while (curDist > moveDist)
                {
                    curDist -= moveDist;
                    beforePosition += moveDist * dir;
                    InvokePosition(beforePosition);
                    moveDist = periodDist;
                    leftDistance = 0f;
                }
                leftDistance += curDist;
                character.transform.position = curPosition;
                prevPosition = curPosition;
            }
        }

        private void InvokePosition(Vector3 position)
        {
            character.transform.position = position;
            Invoke(Me);
        }

        protected override void OnEnd()
        {
            TurnOff();
        }

        private void TurnOn()
        {
            isOn = true;
            prevPosition = character.transform.position;
            leftDistance = periodDist / 2f;
        }

        private void TurnOff()
        {
            isOn = false;
        }
    }
}