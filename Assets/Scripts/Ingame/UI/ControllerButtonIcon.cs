using Rogue.Ingame.Input;
using UnityEngine;

namespace Rogue
{
    public class ControllerButtonIcon : MonoBehaviour
    {
        public GameObject KeyboardObject;
        public GameObject PadObject;
        private ControllerType prevController;
        void Start()
        {
            prevController = ControllerType.KeyboardMouse;
            KeyboardObject.SetActive(true);
            PadObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (InputDetector.CurController != prevController)
            {
                prevController = InputDetector.CurController;
                KeyboardObject.SetActive(prevController == ControllerType.KeyboardMouse);
                PadObject.SetActive(prevController == ControllerType.Pad);
            }
        }
    }
}
