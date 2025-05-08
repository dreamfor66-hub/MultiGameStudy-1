using System.Collections.Generic;
using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class ActionDoFrame
    {
        public ActionData ActionData;
        public int PrevId;
        public int PrevFrame;

        public int LastId;
        public int LastFrame;

        public void ToPrev()
        {
            LastId = PrevId;
            LastFrame = PrevFrame;

            PrevId = 0;
            PrevFrame = 0;
        }

        public void Add(int id, int frame)
        {
            PrevId = LastId;
            PrevFrame = LastFrame;

            LastId = id;
            LastFrame = frame;
        }
    }
    public class CoolTimeModule
    {
        private int frame = 0;
        private readonly List<ActionDoFrame> actionFrames = new List<ActionDoFrame>();

        public int RemainFrame(ActionData actionData, int curFrame)
        {
            var find = Find(actionData);
            if (find == null)
                return 0;
            else
            {
                return Mathf.Max(actionData.CoolTimeFrame - (curFrame - find.LastFrame), 0);
            }
        }

        public void Sync(CharacterStateInfo stateInfo, int serverFrame)
        {
            frame = serverFrame;
            foreach (var frame in actionFrames)
            {
                if (frame.LastId > stateInfo.Id)
                {
                    frame.ToPrev();
                }
            }
            AfterDoCommand(stateInfo);
        }

        public void AfterDoCommand(CharacterStateInfo stateInfo)
        {
            frame++;
            if (stateInfo.ActionData != null)
            {
                var find = Find(stateInfo.ActionData);
                if (find == null)
                    AddAction(stateInfo.ActionData, stateInfo.Id, frame);
                else
                {
                    if (find.LastId < stateInfo.Id)
                    {
                        find.Add(stateInfo.Id, frame);
                    }
                }
            }
        }

        private ActionDoFrame Find(ActionData actionData)
        {
            return actionFrames.Find(x => x.ActionData == actionData);
        }

        private void AddAction(ActionData action, int stateId, int frame)
        {
            actionFrames.Add(new ActionDoFrame
            {
                ActionData = action,
                PrevId = 0,
                PrevFrame = 0,
                LastId = stateId,
                LastFrame = frame,
            });
        }
    }
}