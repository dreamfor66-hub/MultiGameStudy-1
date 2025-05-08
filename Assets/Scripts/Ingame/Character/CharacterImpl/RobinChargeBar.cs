using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue
{
    public class RobinChargeBar : MonoBehaviour
    {
        [SerializeField] Slider ChargeBar;
        [SerializeField] Image BackgroundBar;
        [SerializeField] Image PerfectZoneBar;

        Dictionary<string, int> chargeAnimationKeys = new Dictionary<string, int>()
        {{ "BasicAttack_Down_RE", 0}, { "BasicAttack_Down_PerfectZone", 90},
            {"BasicAttack_Down_loop_start_RE", 100},{"BasicAttack_Down_loop_RE", 130} };
        int MaxFrame => chargeAnimationKeys["BasicAttack_Down_loop_RE"];
        int StartPerfectZoneFrame => chargeAnimationKeys["BasicAttack_Down_PerfectZone"];
        int EndPerfectZoneFrame => chargeAnimationKeys["BasicAttack_Down_loop_start_RE"];


        public void InitChargeBarSetting()
        {
            SetChargeBarSetting(MaxFrame, StartPerfectZoneFrame, EndPerfectZoneFrame);
        }
        public void SetChargeBarSetting(int MaxFrame, int StartPerfectZoneFrame, int EndPerfectZoneFrame)
        {
            ResetValue(MaxFrame);
            SetPerfectZonePosition(MaxFrame, StartPerfectZoneFrame, EndPerfectZoneFrame);
        }

        public void ResetValue(int MaxFrame)
        {
            ChargeBar.maxValue = MaxFrame;
            ChargeBar.value = 0;
        }

        public void SetPerfectZonePosition(float MaxFrame, float StartPerfectZoneFrame, float EndPerfectZoneFrame)
        {
            var backgroundRect = BackgroundBar.rectTransform.rect;
            float MaxWidth = backgroundRect.width;
            var rect = PerfectZoneBar.rectTransform.rect;
            PerfectZoneBar.rectTransform.sizeDelta = new Vector2((MaxWidth / MaxFrame) * (EndPerfectZoneFrame - StartPerfectZoneFrame), rect.height);
            PerfectZoneBar.rectTransform.anchoredPosition = new Vector2(MaxWidth * ((StartPerfectZoneFrame + (EndPerfectZoneFrame - StartPerfectZoneFrame) / 2) / MaxFrame), backgroundRect.height / 2);
        }

        public void SetValueAutomacally(string ActionKey, int Frame)
        {
            if (chargeAnimationKeys.TryGetValue(ActionKey, out var HoldFrame))
            {
                SetValue(HoldFrame + Frame);            }
        }

        public void SetValue(int CurFrame)
        {
            ChargeBar.value = CurFrame;
        }
    }
}
