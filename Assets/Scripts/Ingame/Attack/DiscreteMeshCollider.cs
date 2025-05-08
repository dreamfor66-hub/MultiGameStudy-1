using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Attack
{
    public class DiscreteMeshCollider : AttackCollider
    {
        private List<Collider> hits = new List<Collider>();

        public void OnEnable()
        {

        }

        public void OnDisable()
        {
            hits.Clear();
        }

        public void Update()
        {
            hits.RemoveAll(x => x == null || !x.gameObject.activeInHierarchy);
        }

        public override void Cast(ref List<Hurtbox> output, float prevRatio, float curRatio)
        {
            output.AddRange(hits.Select(x => x.GetComponent<Hurtbox>()).Where(x => x != null));
        }

        public override void MemoryPosition()
        {
        }

        public void OnTriggerEnter(Collider other)
        {
            hits.Add(other);
        }
        public void OnTriggerExit(Collider other)
        {
            hits.Remove(other);
        }
    }
}