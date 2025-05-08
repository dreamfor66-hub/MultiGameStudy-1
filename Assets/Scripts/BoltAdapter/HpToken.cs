using Photon.Bolt;
using UdpKit;

namespace Rogue.BoltAdapter
{
    public class HpToken : PooledProtocolToken
    {
        public int CurHp;
        public int MaxHp;
        public int Shield;

        public override void Read(UdpPacket packet)
        {
            CurHp = packet.ReadInt(16);
            MaxHp = packet.ReadInt(16);
            Shield = packet.ReadInt(16);
        }

        public override void Write(UdpPacket packet)
        {
            packet.WriteInt(CurHp, 16);
            packet.WriteInt(MaxHp, 16);
            packet.WriteInt(Shield, 16);
        }

        public override void Reset()
        {
            CurHp = default;
            MaxHp = default;
            Shield = default;
        }
    }
}