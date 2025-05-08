using Rogue.Ingame.Attack;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class DummyCharacter : MonoBehaviour, IEntity
    {
        public int EntityId => -1;
        public Team Team => Team.None;
        public GameObject GameObject => this.gameObject;

        [SerializeField] private Hurtbox hurtbox;

        void Awake()
        {
            hurtbox.SetEntity(this);
        }

    }
}