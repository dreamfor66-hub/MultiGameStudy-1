using System.Collections.Generic;
using Rogue.Ingame.Character;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI
{
    public class StaminaUI : MonoBehaviour
    {
        [SerializeField] private CharacterBehaviour character;
        [SerializeField] private Image prefab;

        private List<Image> images = new List<Image>();
        private void Start()
        {
            RefreshCount();
            prefab.gameObject.SetActive(false);
        }

        private void RefreshCount()
        {
            while (images.Count < character.StaminaModule.Max)
            {
                var image = Instantiate(prefab, prefab.transform.parent);
                image.gameObject.SetActive(true);
                images.Add(image);
            }

            while (images.Count > character.StaminaModule.Max)
            {
                Destroy(images[images.Count - 1]);
                images.RemoveAt(images.Count - 1);
            }
        }

        private void Update()
        {
            RefreshCount();

            var curStamina = character.StaminaModule.Cur;
            for (var i = 0; i < character.StaminaModule.Max; i++)
            {
                if (i < curStamina)
                {
                    images[i].fillAmount = 1f;
                }
                else if (i == curStamina)
                {
                    images[curStamina].fillAmount = character.StaminaModule.Ratio;
                }
                else
                {
                    images[i].fillAmount = 0f;
                }
            }
        }
    }
}