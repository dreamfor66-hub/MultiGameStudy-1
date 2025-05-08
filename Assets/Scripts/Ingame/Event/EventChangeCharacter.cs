namespace Rogue.Ingame.Event
{
    public struct EventChangeCharacter : IEvent
    {
        public int EntityId;
        public int CharacterNum;

        public EventChangeCharacter(int entityId, int characterNum)
        {
            EntityId = entityId;
            CharacterNum = characterNum;
        }
    }
}