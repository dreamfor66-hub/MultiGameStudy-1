using System.Collections.Generic;
using Photon.Bolt;

namespace Rogue.BoltAdapter
{
    public static class BoltPlayerObjectRegistry
    {
        private static List<BoltPlayerObject> players = new List<BoltPlayerObject>();

        private static BoltPlayerObject CreatePlayer(BoltConnection connection)
        {
            var player = new BoltPlayerObject();
            player.Conneciton = connection;
            if (player.Conneciton != null)
                player.Conneciton.UserData = player;
            players.Add(player);
            return player;
        }

        public static IEnumerable<BoltPlayerObject> AllPlayers => players;
        public static BoltPlayerObject ServerPlayer => players.Find(x => x.IsServer);
        public static int PlayerCount => players.Count;

        public static BoltPlayerObject CreateServerPlayer()
        {
            return CreatePlayer(null);
        }

        public static BoltPlayerObject CreateClientPlayer(BoltConnection connection)
        {
            return CreatePlayer(connection);
        }

        public static void RemovePlayer(BoltConnection connection)
        {
            var player = players.Find(x => x.Conneciton == connection);
            player.Destroy();
            players.Remove(player);
        }

        public static BoltPlayerObject GetPlayer(BoltConnection connection)
        {
            if (connection == null)
                return ServerPlayer;
            return (BoltPlayerObject)connection.UserData;
        }
    }
}