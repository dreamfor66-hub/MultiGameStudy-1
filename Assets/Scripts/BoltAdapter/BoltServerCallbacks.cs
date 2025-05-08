using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UnityEngine;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server, "BoltBase")]
    public class BoltServerCallbacks : GlobalEventListener
    {
        public static bool IsDedicated = false;

        private void Awake()
        {
            BoltPlayerObjectRegistry.CreateServerPlayer();
            BoltNetwork.Instantiate(BoltPrefabs.PF_Bolt_FrameSync);
            BoltNetwork.Instantiate(BoltPrefabs.PF_Bolt_DungeonManager);
        }

        public override void Connected(BoltConnection connection)
        {
            BoltPlayerObjectRegistry.CreateClientPlayer(connection);
        }

        public override void Disconnected(BoltConnection connection)
        {
            BoltPlayerObjectRegistry.RemovePlayer(connection);
        }

        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            BoltPlayerObjectRegistry.ServerPlayer.Spawn();
        }

        public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
        {
            BoltPlayerObjectRegistry.GetPlayer(connection).Spawn();
        }


        public override void OnEvent(PingEvent evnt)
        {
            var conn = evnt.RaisedBy;
            var pong = PongEvent.Create(conn, ReliabilityModes.Unreliable);
            pong.time = evnt.time;
            pong.Send();
        }
    }
}