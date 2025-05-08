using Rogue.Ingame.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Rogue.Ingame.Character.StatusImpl
{
    public class CharacterStatusGhost : ICharacterStatusImpl
    {
        private readonly int originalLayer;
        private readonly GameObject gameObject;
        private readonly NavMeshAgent agent;

        public CharacterStatusGhost(GameObject obj)
        {
            gameObject = obj;
            originalLayer = obj.layer;
            agent = obj.GetComponent<NavMeshAgent>();
        }

        public void TurnOn()
        {
            gameObject.layer = LayerHelper.GhostLayer;
            if (agent != null)
                agent.enabled = false;
        }

        public void TurnOff()
        {
            gameObject.layer = originalLayer;
            if (agent != null)
                agent.enabled = true;
        }
    }
}