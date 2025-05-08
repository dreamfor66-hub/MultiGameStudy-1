using Sirenix.OdinInspector;
using UnityEngine;

namespace FMLib.UI.OnOff
{
    public class OnOffActive : OnOffBehaviour
    {
        [SerializeField] [Required] private GameObject onObj;
        [SerializeField] [Required] private GameObject offObj;
        public override void On()
        {
            onObj.SetActive(true);
            offObj.SetActive(false);
        }

        public override void Off()
        {
            onObj.SetActive(false);
            offObj.SetActive(true);
        }
    }
}