using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Attack
{
    public class Hurtbox : MonoBehaviour
    {
        public Vector3 center;
        public bool NowParrying { get; private set; } = false;
        public bool NowSuperArmor => defaultSuperArmor || statusSuperArmor;
        public Team Team => Entity?.Team ?? Team.None;

        public IEntity Entity { get; private set; }
        public Vector3 Position => transform.TransformPoint(center);

        private bool defaultSuperArmor;
        private bool statusSuperArmor;

        public void SetEntity(IEntity entity)
        {
            this.Entity = entity;
        }

        public void SetParrying(bool parrying)
        {
            NowParrying = parrying;
        }

        public void SetDefaultSuperArmor(bool value)
        {
            defaultSuperArmor = value;
        }

        public void SetStatusSuperArmor(bool value)
        {
            statusSuperArmor = value;
        }
    }
}