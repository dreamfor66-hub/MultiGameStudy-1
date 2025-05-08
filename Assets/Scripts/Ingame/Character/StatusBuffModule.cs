using System;
using System.Collections.Generic;
using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class StatusBuffInfo
    {
        public CharacterStatusType Type;
        public int Frame;

        public StatusBuffInfo(CharacterStatusType type, int frame)
        {
            Type = type;
            Frame = frame;
        }
    }

    public class StatusBuffModule
    {
        public readonly List<StatusBuffInfo> StatusBuffs;
        public StatusBuffModule()
        {
            StatusBuffs = new List<StatusBuffInfo>();
        }

        public void UpdateFrame()
        {
            foreach (var item in StatusBuffs)
                item.Frame--;
            StatusBuffs.RemoveAll(x => x.Frame <= 0);
        }

        public void Add(CharacterStatusType type, int frame)
        {
            StatusBuffs.Add(new StatusBuffInfo(type, frame));
        }

    }
}