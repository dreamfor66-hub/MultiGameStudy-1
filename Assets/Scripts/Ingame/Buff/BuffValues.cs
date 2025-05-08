using System;
using System.Collections.Generic;
using Rogue.Ingame.Data.Buff;

namespace Rogue.Ingame.Buff
{
    public class BuffValues
    {
        public int MoveSpeedPercent => GetValue(BuffSimpleValueType.MoveSpeedPercent);
        public int AttackSpeedPercent => GetValue(BuffSimpleValueType.AttackSpeedPercent);
        public bool Freeze => GetValue(BuffSimpleValueType.Freeze) > 0;
        public bool Hide => GetValue(BuffSimpleValueType.Hide) > 0;

        private readonly Dictionary<BuffSimpleValueType, int> dict = new Dictionary<BuffSimpleValueType, int>();
        private readonly Dictionary<BuffSimpleValueType, Action<int>> actions = new Dictionary<BuffSimpleValueType, Action<int>>();

        public void AddValue(BuffSimpleValueType type, int value)
        {
            if (value == 0)
                return;
            dict[type] = GetValue(type) + value;
            CallAction(type);
        }

        public int GetValue(BuffSimpleValueType type)
        {
            return !dict.ContainsKey(type) ? 0 : dict[type];
        }

        public void SyncValue(BuffSimpleValueType type, int value)
        {
            if (GetValue(type) == value)
                return;
            dict[type] = value;
            CallAction(type);
        }

        private void CallAction(BuffSimpleValueType type)
        {
            if (actions.ContainsKey(type))
                actions[type]?.Invoke(GetValue(type));
        }

        public void Subscribe(BuffSimpleValueType type, Action<int> action, bool callCurrent)
        {
            if (!actions.ContainsKey(type))
                actions[type] = null;

            actions[type] += action;

            if (callCurrent)
                action.Invoke(GetValue(type));
        }

        public void UnSubscribe(BuffSimpleValueType type, Action<int> action)
        {
            actions[type] -= action;
        }
    }

}