using Rogue.Ingame.Buff;
using Rogue.Ingame.Data;
using Rogue.Ingame.Data.Buff;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Status
{
    public class BuffIconView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI time;
        [SerializeField] private TextMeshProUGUI stack;

        private int duration;
        private int stackCount;
        private float remainTime;
        private int TimeValue => Mathf.FloorToInt(remainTime);

        public void UpdateInfo(BuffInfo buffInfo)
        {
            icon.sprite = buffInfo.BuffData.IconData.Icon;
            duration = buffInfo.DurationFrame;
            stackCount = buffInfo.StackCount;
            remainTime = (float)buffInfo.RemainFrame / CommonVariables.GameFrame;

            OnValueChanged();
        }

        public void OnLateUpdate()
        {
            var prevTime = TimeValue;
            remainTime -= Time.deltaTime;
            if (prevTime != TimeValue)
            {
                OnValueChanged();
            }
        }

        private void OnValueChanged()
        {
            time.text = TimeValue <= 0 ? "" : $"{TimeValue}";
            stack.text = stackCount > 0 ? stackCount.ToString() : "";
        }

        public void clear()
        {
            icon.sprite = null;
            time.text = null;
            stack.text = null;
        }
    }
}