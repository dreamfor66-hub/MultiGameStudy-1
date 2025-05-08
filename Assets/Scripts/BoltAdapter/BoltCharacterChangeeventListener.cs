using Photon.Bolt;
using Rogue.Ingame.Event;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour]
    public class BoltCharacterChangeEventListener : GlobalEventListener
    {
        private void Awake()
        {
            EventDispatcher.Listen<EventChangeCharacter>(OnEventChangeCharacter);
        }

        private void OnDestroy()
        {
            EventDispatcher.Remove<EventChangeCharacter>(OnEventChangeCharacter);
        }

        private void OnEventChangeCharacter(EventChangeCharacter evt)
        {
            var boltEvent = CharacterChangeEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);
            boltEvent.EntityId = evt.EntityId;
            boltEvent.CharacterNum = evt.CharacterNum;
            boltEvent.Send();
        }

        public override void OnEvent(CharacterChangeEvent evnt)
        {
            foreach (var player in BoltPlayerObjectRegistry.AllPlayers)
            {
                if (player.Character.playerCharacter.EntityId == evnt.EntityId)
                    player.ChangeCharacter(evnt.CharacterNum);
            }
        }

    }
}
