using System.Collections;
using Rogue.Ingame.Character;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI
{
    public class HpUI : MonoBehaviour
    {
        [SerializeField] private CharacterBehaviour character;

        [SerializeField] private GameObject parent;
        [SerializeField] private Image redBar;
        [SerializeField] private Image hpBar;
        [SerializeField] private RectTransform whiteBar;

        public void Reset()
        {
            character = GetComponentInParent<CharacterBehaviour>();
        }

        private int prevHp;
        private int prevShield;
        private bool first = true;
        private Coroutine redAnimCoroutine;
        private HpModule hpModule;

        private void Start()
        {
            parent.SetActive(false);
            hpModule = character.HpModule;
            SetShield(0f);
        }

        private void LateUpdate()
        {
            var info = hpModule.HpInfo;
            var ratio = info.Ratio;
            hpBar.fillAmount = ratio;

            if (first)
            {
                prevHp = info.CurHp;
                prevShield = 0;
                redBar.fillAmount = ratio;
                first = false;
            }
            else if (prevHp != info.CurHp)
            {
                parent.SetActive(true);
                if (redAnimCoroutine != null)
                    StopCoroutine(redAnimCoroutine);
                redAnimCoroutine = StartCoroutine(RedAnimation(redBar.fillAmount, ratio));
            }

            UpdateShield();

            prevHp = info.CurHp;
        }


        private IEnumerator RedAnimation(float startRatio, float endRatio)
        {
            redBar.fillAmount = startRatio;
            yield return new WaitForSeconds(.5f);

            var time = 0f;
            const float reduceTime = .3f;
            while (time < reduceTime)
            {
                var t = time / reduceTime;
                var ratio = Mathf.Lerp(startRatio, endRatio, t);
                redBar.fillAmount = ratio;
                yield return null;
                time += Time.deltaTime;
            }

            redBar.fillAmount = endRatio;
            if (prevHp == 0)
            {
                parent.SetActive(false);
            }
            redAnimCoroutine = null;
        }

        public void SetModule(HpModule hpModule)
        {
            this.hpModule = hpModule;
        }

        private void UpdateShield()
        {
            var info = hpModule.HpInfo;
            var curShield = info.Shield;
            if (curShield != prevShield)
            {
                parent.SetActive(true);
                SetShield((float)curShield / info.MaxHp);
            }
            prevShield = curShield;
        }

        private void SetShield(float ratio)
        {
            var hpRatio = hpModule.HpInfo.Ratio;
            if (ratio >= 1f)
            {
                SetWhiteBar(0f, 1f);
            }
            else if (ratio >= 1 - hpRatio)
            {
                SetWhiteBar(1 - ratio, 1f);
            }
            else
            {
                SetWhiteBar(hpRatio, hpRatio + ratio);
            }
        }

        private void SetWhiteBar(float min, float max)
        {
            whiteBar.anchorMin = new Vector2(min, whiteBar.anchorMin.y);
            whiteBar.anchorMax = new Vector2(max, whiteBar.anchorMax.y);
            whiteBar.offsetMin = new Vector2(0, whiteBar.offsetMin.y);
            whiteBar.offsetMax = new Vector2(0, whiteBar.offsetMax.y);
        }
    }
}