using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

namespace Rogue.Ingame.UI
{
    public class BillBoard : MonoBehaviour
    {
        private UnityEngine.Camera cam;
        [SerializeField] bool PosToCamera;
        [SerializeField, ShowIf("PosToCamera")] Vector3 PosOffset;

        private void Start()
        {
            cam = UnityEngine.Camera.main;
        }

        private void LateUpdate()
        {
            if (cam != null)
            {
                transform.forward = cam.transform.forward;
                if (PosToCamera)
                    transform.position = transform.parent.position + cam.transform.right * PosOffset.x + cam.transform.up * PosOffset.y + cam.transform.forward * PosOffset.z;
            }
        }
    }
}