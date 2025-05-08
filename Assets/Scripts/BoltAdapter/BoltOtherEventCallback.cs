using Photon.Bolt;
using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Entity;
using Rogue.Ingame.Event;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour]
    public class BoltOtherEventCallback : GlobalEventListener
    {
        public override void OnEvent(HealEvent evnt)
        {
            var healInfo = CreateHealInfo(evnt);
            if (healInfo.Target == null)
                return;
            var position = healInfo.Target.GameObject.transform.position + new Vector3(0f, 1.5f, 0f) +
                           Random.insideUnitSphere;
            EventDispatcher.Send(new EventHeal(healInfo.RealAmount, position));
        }


        private HealResultInfo CreateHealInfo(HealEvent evnt)
        {
            HealResultInfo info;
            info.Target = EntityTable.FindById(evnt.Target);
            info.RealAmount = evnt.RealAmount;
            return info;
        }
    }
}