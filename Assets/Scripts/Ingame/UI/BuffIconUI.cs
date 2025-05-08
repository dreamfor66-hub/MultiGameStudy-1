using Rogue.Ingame.Buff;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI
{
    public class BuffIconUI : MonoBehaviour
    {
        [SerializeField] private Text stackText;
        [SerializeField] private Image iconImage;

        private bool showStack;

        public BuffInstance Buff { get; private set; }


        public void Set(Sprite icon, BuffInstance buff, bool showStack)
        {
            iconImage.sprite = icon;
            this.Buff = buff;
            this.showStack = showStack;
            stackText.gameObject.SetActive(showStack);
            stackText.text = buff.StackCount.ToString();
        }

        public void Clear()
        {
            Buff = null;
        }

        private void Update()
        {
            if (showStack && Buff != null)
                stackText.text = Buff.StackCount.ToString();
        }
    }
}