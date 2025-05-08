using Rogue.Ingame.Attack.Struct;
using Rogue.Ingame.Data;

namespace Rogue.Ingame.Bullet
{
    public class DpModule
    {
        private readonly DpData data;
        private int dp = 0;
        public DpModule(DpData data)
        {
            this.data = data;
        }

        public void Reset()
        {
            if (!data.Enabled)
                return;
            dp = data.MaxDp;
        }

        public bool Hurt(HitResultInfo hitInfo)
        {
            if (!data.Enabled)
                return false;

            if (dp <= 0)
                return false;

            dp -= hitInfo.Damage.DpDamage;
            if (dp <= 0)
            {
                dp = 0;
                return true;
            }
            else
                return false;
        }
    }
}