using Rogue.Device.Vibration;
using Rogue.Ingame.Event;
using UnityEngine;

namespace Rogue.Ingame.Implement
{
    public class EventToVibration : MonoBehaviour
    {
        public void Start()
        {
            EventDispatcher.Listen<EventAttackHit>(OnEventAttackHit);
        }

        public void Destroy()
        {
            EventDispatcher.Remove<EventAttackHit>(OnEventAttackHit);
        }

        private void OnEventAttackHit(EventAttackHit evt)
        {
            if (evt.AttackerIsMe || evt.VictimIsMe)
                VibrationGenerator.High(evt.HitInfo.Main.HitFx.VibrationTime);
        }
    }
}