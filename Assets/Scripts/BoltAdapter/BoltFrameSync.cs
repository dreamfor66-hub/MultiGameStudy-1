using Photon.Bolt;
using Rogue.Ingame.Network;

namespace Rogue.BoltAdapter
{
    public class BoltFrameSync : EntityBehaviour<IFrameState>
    {
        public static int Frame => instance.GetFrame();
        private static BoltFrameSync instance;

        private int GetFrame() => entity.IsOwner ? BoltNetwork.ServerFrame : frameDeJitter.Frame;
        private readonly FrameDeJitter frameDeJitter = new FrameDeJitter();

        public void Awake()
        {
            instance = this;
        }

        public void OnDestroy()
        {
            instance = null;
        }

        public override void Attached()
        {
            state.AddCallback("ServerFrame", ServerFrameChanged);
        }

        private void ServerFrameChanged()
        {
            if (entity.IsOwner)
                return;
            else
                frameDeJitter.Set(state.ServerFrame);
        }

        private void FixedUpdate()
        {
            if (entity.IsOwner)
                state.ServerFrame = BoltNetwork.ServerFrame;
            else
                frameDeJitter.UpdateFrame();
            ServerFrameHolder.Frame = GetFrame();
        }
    }
}
