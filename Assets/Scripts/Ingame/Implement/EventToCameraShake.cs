using Rogue.Ingame.Camera;
using Rogue.Ingame.Event;
using UnityEngine;

namespace Rogue.Ingame.Implement
{
    public class EventToCameraShake : MonoBehaviour
    {
        private CameraShake camShake;
        public void Start()
        {
            camShake = FindObjectOfType<CameraShake>();
            EventDispatcher.Listen<EventAttackHit>(OnEventAttackHit);
        }

        public void Destroy()
        {
            EventDispatcher.Remove<EventAttackHit>(OnEventAttackHit);
        }

        private void OnEventAttackHit(EventAttackHit evt)
        {
            if (camShake == null)
                return;
            if (evt.AttackerIsMe || evt.VictimIsMe)
                camShake.Shake(evt.HitInfo.Main.HitFx.CameraShakeLength);
        }
    }
}
