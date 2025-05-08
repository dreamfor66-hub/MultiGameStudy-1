using FMLib.Structs;
using Photon.Bolt;
using Rogue.Ingame.Bullet;
using Rogue.Ingame.Entity;

namespace Rogue.BoltAdapter
{
    [BoltGlobalBehaviour(BoltNetworkModes.Client)]
    public class BoltBulletEventClient : GlobalEventListener
    {
        public override void OnEvent(SpawnBulletEvent evnt)
        {
            var spawnInfo = new BulletSpawnInfo
            {
                EntityId = evnt.UniqueId,
                Position = evnt.Position,
                Velocity = evnt.Velocity,
                Frame = evnt.ServerFrame,
                RootSourceId = evnt.RootSourceId
            };
            BoltBulletManager.Instance.SpawnClient(evnt.TableId, evnt.UniqueId, spawnInfo);
        }
    }
}