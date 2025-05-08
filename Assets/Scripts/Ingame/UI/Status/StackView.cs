using System.Collections.Generic;
using Rogue.Ingame.Character;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Status
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class StackView : MonoBehaviour
    {
        [SerializeField] private Image prefab;
        private Color defaultColor;
        private Color emptyColor;

        private StackModule stackModule;
        [SerializeField] private List<Image> stackImages = new List<Image>();

        private void Start()
        {
            ColorChange();
        }

        void ColorChange()
        {
            defaultColor = prefab.color;
            emptyColor = defaultColor;
            defaultColor.a = 1f;
            emptyColor.a = 0f;
        }

        public void Bind(StackModule stack)
        {
            stackModule = stack;
            Init();
        }

        public void Release()
        {
            stackModule = null;
        }
        private void Init()
        {
            //매번 List를 초기화.
            stackImages.Clear();

            //maxStack만큼 child를 탐색.
            for (int i = 0; i < stackModule.MaxStack; i++)
            {
                //childCount가 maxStack까지 안왔을때.
                if (transform.childCount <= i)
                {
                    var image = Instantiate(prefab, prefab.transform.parent);
                    stackImages.Add(image);
                }
                else
                {
                    stackImages.Add(transform.GetChild(i).GetComponent<Image>());
                }
                stackImages[i].gameObject.SetActive(true);
            }
            // maxStack초과 갯수의 오브젝트는 지운다.
            while (transform.childCount > stackModule.MaxStack)
            {
                int LastIndex = transform.childCount - 1;
                DestroyImmediate(transform.GetChild(LastIndex).gameObject);
            }
        }

        private void LateUpdate()
        {
            if (stackModule == null)
            {
                return;
            }

            for (var i = 0; i < stackImages.Count; i++)
            {
                stackImages[i].color = (i < stackModule.Info.CurStack) ? defaultColor : emptyColor;
            }
        }
    }
}