namespace FMLib.UI.OnOff
{
    public class OnOffActiveSelf : OnOffBehaviour
    {
        public override void On()
        {
            gameObject.SetActive(true);
        }

        public override void Off()
        {
            gameObject.SetActive(false);
        }
    }
}