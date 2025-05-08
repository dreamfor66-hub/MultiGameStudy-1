using System;
using Rogue.Ingame.Data.Buff;

namespace Rogue.Ingame.Buff
{
    public class BuffEffectSimpleValue
    {
        private readonly BuffEffectSimpleData data;
        private readonly BuffCondition buffCondition;
        private readonly BuffValues buffValues;
        private int stackCount;
        private bool isOn;
        private int lastAppliedValue;
        private readonly Func<float> curveFunction;

        public BuffEffectSimpleValue(BuffEffectSimpleData data, BuffCondition buffCondition, BuffValues buffValues)
        {
            this.data = data;
            this.buffCondition = buffCondition;
            this.buffValues = buffValues;
            stackCount = 1;
            isOn = false;
            lastAppliedValue = 0;
            curveFunction = CurveFunc();
        }

        public void OnStart()
        {
            var value = CalculateValue();
            buffValues.AddValue(data.ValueType, value);
            lastAppliedValue = value;
            isOn = true;
        }

        public void OnUpdate()
        {
            if (isOn)
                UpdateValues();
        }

        public void OnStackCountChanged(int newStack)
        {
            stackCount = newStack;
            if (isOn)
                UpdateValues();
        }

        public void OnEnd()
        {
            buffValues.AddValue(data.ValueType, -lastAppliedValue);
            lastAppliedValue = 0;
            isOn = false;
        }

        private void UpdateValues()
        {
            var newValue = CalculateValue();
            if (newValue == lastAppliedValue)
                return;

            buffValues.AddValue(data.ValueType, newValue - lastAppliedValue);
            lastAppliedValue = newValue;
        }


        private int CalculateValue()
        {
            var curveValue = curveFunction();
            switch (data.StackType)
            {
                case BuffSimpleStackType.None:
                    return (int)(data.Value * curveValue);
                case BuffSimpleStackType.Multiply:
                    return (int)(data.Value * stackCount * curveValue);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Func<float> CurveFunc()
        {
            switch (data.CurveType)
            {
                case BuffSimpleCurveType.None:
                    return () => 1f;
                case BuffSimpleCurveType.HpRatio:
                    return () => data.Curve.Evaluate(buffCondition.HpRatio);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}