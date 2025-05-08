using Photon.Bolt;
using UdpKit;

namespace Rogue.BoltAdapter
{
    public class StageStateToken : PooledProtocolToken
    {
        public int Idx;
        public string SceneName;

        public override void Read(UdpPacket packet)
        {
            Idx = packet.ReadInt(8);
            SceneName = packet.ReadString();
        }

        public override void Write(UdpPacket packet)
        {
            packet.WriteInt(Idx, 8);
            packet.WriteString(SceneName);
        }

        public override void Reset()
        {
            Idx = 0;
            SceneName = "";
        }
    }
}