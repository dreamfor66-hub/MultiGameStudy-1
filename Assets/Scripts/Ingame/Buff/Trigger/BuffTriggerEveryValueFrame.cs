using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using UnityEngine;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerEveryValueFrame : BuffTriggerBase
    {
        private readonly int periodFrame;
        private int curFrame;
        public BuffTriggerEveryValueFrame(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
            periodFrame = triggerData.Value;
        }

        protected override void OnStart()
        {
            curFrame = 0;
        }

        protected override void OnUpdate()
        {
            curFrame++;
            if (curFrame >= periodFrame)
            {
                Invoke(Me);
                curFrame -= periodFrame;
            }
        }

        protected override void OnEnd()
        {
        }
    }
}