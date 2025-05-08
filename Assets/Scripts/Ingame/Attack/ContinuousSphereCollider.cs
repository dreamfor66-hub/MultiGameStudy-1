using System.Collections.Generic;
using System.Text;
using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Attack
{
    public class ContinuousSphereCollider : AttackCollider
    {
        [SerializeField] public float Radius;

        private Vector3 prevPosition;
        private Vector3 curPosition;

        private readonly RaycastHit[] hits = new RaycastHit[1024];

        private int lastCastFrame;

        private Vector3 prevCalculated;
        private Vector3 curCalculated;

        public override void Cast(ref List<Hurtbox> output, float prevRatio, float curRatio)
        {
            lastCastFrame = Time.frameCount;
            var scale = transform.lossyScale;
            var radius = Mathf.Max(scale.x, scale.y, scale.z) * Radius;
            prevCalculated = Vector3.Lerp(prevPosition, curPosition, prevRatio);
            curCalculated = Vector3.Lerp(prevPosition, curPosition, curRatio);
            var hitCount = Physics.CapsuleCastNonAlloc(prevCalculated, curCalculated, radius, Vector3.up, hits, 0f, LayerHelper.HurtboxMask);

            for (var i = 0; i < hitCount; i++)
            {
                var hit = hits[i];
                var hurtbox = hit.collider.GetComponent<Hurtbox>();
                output.Add(hurtbox);
            }
        }

        public override void MemoryPosition()
        {
            prevPosition = curPosition;
            curPosition = transform.position;
        }

        private void OnDrawGizmos()
        {
            var tm = transform;
            var scale = tm.lossyScale;
            var center = tm.position;
            var radius = Mathf.Max(scale.x, scale.y, scale.z) * Radius;
            Gizmos.DrawWireSphere(center, radius);

            if (Application.isPlaying && lastCastFrame == Time.frameCount)
            {
                Gizmos.DrawSphere(prevCalculated, radius);
                Gizmos.DrawSphere(curCalculated, radius);
            }
        }
    }
}