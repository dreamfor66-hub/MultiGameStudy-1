using System.Collections.Generic;
using UnityEngine;

namespace Rogue.Ingame.Attack
{
    public abstract class AttackCollider : MonoBehaviour
    {
        public abstract void Cast(ref List<Hurtbox> output, float prevRatio, float curRatio);
        public abstract void MemoryPosition();
    }
}