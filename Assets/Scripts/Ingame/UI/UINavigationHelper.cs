using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rogue.Ingame.UI
{
    public class UINavigationHelper : MonoBehaviour
    {
        private GameObject lastSelected;
        private IEnumerable<GameObject> selectable;

        public void SetSelectable(IEnumerable<GameObject> selectable)
        {
            this.selectable = selectable;
            lastSelected = null;
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (lastSelected != null)
                    EventSystem.current.SetSelectedGameObject(lastSelected);
                else if (selectable != null && selectable.Any())
                    EventSystem.current.SetSelectedGameObject(selectable.First());
            }
            else
            {
                lastSelected = EventSystem.current.currentSelectedGameObject;
            }
        }
    }
}
