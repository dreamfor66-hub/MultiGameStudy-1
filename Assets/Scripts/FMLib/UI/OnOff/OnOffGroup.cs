using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FMLib.UI.OnOff
{
    public class OnOffGroup : OnOffBehaviour
    {
        [SerializeField] private List<OnOffBehaviour> onOffs;

        [Button]
        private void FindChildren()
        {
            onOffs = GetComponentsInChildren<OnOffBehaviour>(true).ToList();
            onOffs.Remove(this);
        }

        public override void On()
        {
            foreach (var onOff in onOffs)
                onOff.On();
        }

        public override void Off()
        {
            foreach (var onOff in onOffs)
                onOff.Off();
        }
    }
}