using FMLib.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Camera
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField]
        private float distToAcc = 3600f;
        [SerializeField]
        [Range(0, 1)]
        private float damp = 0.3f;
        [SerializeField]
        private float velSize = 1f;
        [SerializeField]
        private float velAngle = 120f;

        [SerializeField]
        private float maxDeltaTime = 0.005f;

        private Vector3 vel = Vector3.zero;

        private void Update()
        {
            var remainDelta = Time.deltaTime;
            while (remainDelta > 0)
            {
                var deltaTime = remainDelta > maxDeltaTime ? maxDeltaTime : remainDelta;
                UpdateTime(deltaTime);
                remainDelta -= deltaTime;
            }
        }

        private void UpdateTime(float deltaTime)
        {
            var acc = -transform.localPosition * distToAcc;
            this.vel += acc * deltaTime;
            vel = vel * (1 - damp);
            transform.localPosition += vel * deltaTime;
        }

        [Button]
        public void Shake(float length)
        {
            var angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            var vec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * length;

            var coEff = Mathf.Sqrt(Mathf.Abs(distToAcc));
            var newVel = vec * coEff * velSize;
            vel = newVel.Rotate(Mathf.Deg2Rad * velAngle);
            transform.localPosition = vec;
        }
    }
}