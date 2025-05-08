using Rogue.Ingame.Attack.Struct;
using UnityEngine;

namespace Rogue.Ingame.Entity
{
    public interface IEntity
    {
        public int EntityId { get; }
        public Team Team { get; }
        public GameObject GameObject { get; }
    }
}