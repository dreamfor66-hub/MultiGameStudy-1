using Sirenix.OdinInspector;
using UnityEngine;

namespace FMLib.UI.OnOff
{
    public abstract class OnOffBehaviour : MonoBehaviour
    {
        [Button]
        public abstract void On();

        [Button]
        public abstract void Off();

        public void Set(bool onOff)
        {
            if (onOff)
                On();
            else
                Off();
        }
    }
}