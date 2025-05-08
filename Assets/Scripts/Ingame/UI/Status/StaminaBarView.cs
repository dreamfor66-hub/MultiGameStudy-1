using System.Collections.Generic;
using Rogue.Ingame.Character;
using Rogue.Ingame.Util.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Status
{
    public class StaminaBarView : MonoBehaviour
    {
        [SerializeField] private Image prefab;

        private readonly List<Image> bars = new List<Image>();
        private StaminaModule stamina;

        private void Awake()
        {
        }

        public void Bind(StaminaModule staminaModule)
        {
            stamina = staminaModule;
            RefreshCount();
        }

        public void Release()
        {
            stamina = null;
        }


        private void RefreshCount()
        {
            while (bars.Count < stamina.Max)
            {
                var image = Instantiate(prefab, prefab.transform.parent);
                image.gameObject.SetActive(true);
                bars.Add(image);
            }
            while (bars.Count > stamina.Max)
            {
                Destroy(bars[bars.Count - 1].gameObject);
                bars.RemoveAt(bars.Count - 1);
            }
        }

        private void LateUpdate()
        {
            if (stamina == null)
                return;

            RefreshCount();
            for (var i = 0; i < stamina.Max; i++)
            {
                if (i < stamina.Cur)
                    bars[i].fillAmount = 1f;
                else if (i == stamina.Cur)
                    bars[i].fillAmount = stamina.Ratio;
                else
                    bars[i].fillAmount = 0f;
            }
            var cur = stamina.Cur;

        }


    }
}