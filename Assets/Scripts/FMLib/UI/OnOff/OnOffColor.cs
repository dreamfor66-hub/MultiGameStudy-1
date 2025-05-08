using UnityEngine;
using UnityEngine.UI;

namespace FMLib.UI.OnOff
{
    [RequireComponent(typeof(Graphic))]
    public class OnOffColor : OnOffBehaviour
    {
        [SerializeField] protected Graphic graphic;
        [SerializeField] private Color onColor = Color.white;
        [SerializeField] private Color offColor = Color.gray;

        private void Reset()
        {
            graphic = GetComponent<Graphic>();
        }

        public override void On()
        {
            graphic.color = onColor;
        }

        public override void Off()
        {
            graphic.color = offColor;
        }
    }
}