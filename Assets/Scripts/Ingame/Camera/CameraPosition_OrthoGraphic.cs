using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Camera
{
    public class CameraPosition_OrthoGraphic : MonoBehaviour
    {
        [SerializeField] private float distance = 15f;
        [SerializeField] private float angle = 45f;
        [SerializeField] private float minSize = 7f;

        [SerializeField] private UnityEngine.Camera cam;

        public void Start()
        {
            prevPos = transform.position;
            prevSize = cam.orthographicSize;
        }

        private Vector3 prevPos;
        private float prevSize;

        [Button]
        private void Refresh()
        {
            var targetPos = Vector3.zero;
            var finalSize = minSize;
            var targets = CameraTarget.Targets;
            if (targets.Count > 0)
            {
                var min = targets.Select(x => x.position).Aggregate(Vector3.Min);
                var max = targets.Select(x => x.position).Aggregate(Vector3.Max);
                var center = (min + max) / 2f;
                var margin = 8f;
                var xSize = (max - min).x + margin;
                var zSize = (max - min).z + margin;
                center.y = 0f;
                targetPos = center;
                finalSize = Mathf.Max(xSize / cam.aspect / 2f, zSize * Mathf.Sin(angle) / 2f, minSize);

            }

            var look = Quaternion.AngleAxis(angle, Vector3.right) * Vector3.forward;
            var rot = Quaternion.LookRotation(look, Vector3.up);
            var finalPos = targetPos - look * distance;

            if (Application.isPlaying)
            {
                var curSize = Mathf.Lerp(prevSize, finalSize, 0.1f);
                var curPos = Vector3.Lerp(prevPos, finalPos, 0.1f);

                transform.SetPositionAndRotation(curPos, rot);
                cam.orthographicSize = curSize;

                prevPos = curPos;
                prevSize = curSize;
            }
            else
            {
                transform.SetPositionAndRotation(finalPos, rot);
                cam.orthographicSize = finalSize;
            }
        }

        private void LateUpdate()
        {
            Refresh();
        }
    }
}
