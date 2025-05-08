using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;
using Rogue.Ingame.GameCommand;

namespace Rogue.Ingame.Character
{
    public class CharacterBuff
    {
        private readonly IEntity entity;

        public CharacterBuff(IEntity entity)
        {
            this.entity = entity;
        }

        public void UpdateBuff(CharacterStateUpdateInfo update)
        {
            if (update.Cur.StateType == CharacterStateType.Action)
            {
                var actionData = update.Cur.ActionData;
                var curFrame = update.Cur.Frame;
                var prevFrame = curFrame - update.DeltaFrame;
                if (update.IsStateChanged) // 0프레임 버프가 안걸리는 문제 수정.
                {
                    prevFrame = -0.1f;
                }
                UpdateAction(actionData, prevFrame, curFrame);
            }
        }

        private void UpdateAction(ActionData action, float prevFrame, float curFrame)
        {
            foreach (var buff in action.BuffData)
            {
                if (prevFrame < buff.Frame && buff.Frame <= curFrame)
                {
                    AddBuff(buff.Buff);
                }
            }
        }

        private void AddBuff(BuffData buff)
        {
            GameCommandAddBuff.Send(entity, entity, buff);
        }
    }
}