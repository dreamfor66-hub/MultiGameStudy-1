using System.Collections.Generic;
using UnityEngine;

namespace Rogue.Ingame.Attack
{
    public class HitColliderAnchor : MonoBehaviour
    {
        public AttackCollider[] Colliders => colliders;

        public int Id;

        private AttackCollider[] colliders;

        private void Awake()
        {
            colliders = GetComponentsInChildren<AttackCollider>();
        }

        public void Cast(ref List<Hurtbox> output, float prevRatio, float curRatio)
        {
            foreach (var col in colliders)
            {
                if (col.enabled)
                    col.Cast(ref output, prevRatio, curRatio);
            }
        }

        public void MemoryPosition()
        {
            foreach (var col in colliders)
                col.MemoryPosition();
        }
    }
}