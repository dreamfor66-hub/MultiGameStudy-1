using DG.Tweening;
using Rogue.Ingame.Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Status
{
    public class HpBarView : MonoBehaviour
    {
        public bool IsDead => redBar.fillAmount == 0;
        [SerializeField] private Image hpBar;
        [SerializeField] private Image redBar;
        [SerializeField] private RectTransform shieldBar;
        [SerializeField] private TextMeshProUGUI text;

        private HpModule hpModule;

        private int prevHp = 0;
        private int prevShield = 0;

        public void Bind(HpModule hp)
        {
            hpModule = hp;
            Init();
        }

        public void Release()
        {
            hpModule = null;
        }
        private void Init()
        {
            var info = hpModule.HpInfo;
            hpBar.fillAmount = info.Ratio;
            redBar.fillAmount = info.Ratio;
            prevHp = info.CurHp;
            prevShield = info.Shield;
            SetShield(0, 0);
        }

        private void LateUpdate()
        {
            if (hpModule == null)
                return;
            var info = hpModule.HpInfo;
            var hpRatio = info.Ratio;
            hpBar.fillAmount = hpRatio;
            text.text = $"{info.CurHp}/{info.MaxHp}";
            if (prevHp != info.CurHp)
            {
                redBar.DOFillAmount(hpRatio, .3f);
                prevHp = info.CurHp;
            }

            if (prevShield != info.Shield)
            {
                var shieldRatio = (float)info.Shield / info.MaxHp;
                SetShield(hpRatio, shieldRatio);
                prevShield = info.Shield;
            }
        }

        private void SetShield(float hpRatio, float shieldRatio)
        {
            var max = Mathf.Min(hpRatio + shieldRatio, 1f);
            var min = Mathf.Max(max - shieldRatio, 0f);
            shieldBar.anchorMin = new Vector2(min, shieldBar.anchorMin.y);
            shieldBar.anchorMax = new Vector2(max, shieldBar.anchorMax.y);
            shieldBar.offsetMin = new Vector2(0, shieldBar.offsetMin.y);
            shieldBar.offsetMax = new Vector2(0, shieldBar.offsetMax.y);
        }
    }
}
