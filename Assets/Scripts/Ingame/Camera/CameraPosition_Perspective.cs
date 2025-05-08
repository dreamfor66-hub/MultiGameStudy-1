using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.Camera
{
    public class CameraPosition_Perspective : MonoBehaviour
    {
        [SerializeField] private float minDistance = 7f;
        [SerializeField] private float angle = 45f;
        [SerializeField] private UnityEngine.Camera cam;

        private float pivotDist;
        private Vector3 pivotTopLeft;
        private Vector3 pivotTopRight;
        private Vector3 pivotBottomLeft;
        private Vector3 pivotBottomRight;

        private void Start()
        {
            SetRaw(Vector3.zero, minDistance);
            CalculatePivots(minDistance);
        }


        private void SetRaw(Vector3 targetPos, float dist)
        {
            var look = Quaternion.AngleAxis(angle, Vector3.right) * Vector3.forward;
            var rot = Quaternion.LookRotation(look, Vector3.up);
            var camPos = targetPos - look * dist;
            transform.SetPositionAndRotation(camPos, rot);
        }

        private void CalculatePivots(float curDist)
        {
            pivotDist = curDist;
            pivotBottomLeft = GetPivotPoint(Vector3.zero);
            pivotBottomRight = GetPivotPoint(new Vector3(Screen.width, 0));
            pivotTopLeft = GetPivotPoint(new Vector3(0, Screen.height));
            pivotTopRight = GetPivotPoint(new Vector3(Screen.width, Screen.height));
        }

        private Vector3 GetPivotPoint(Vector3 screenPosition)
        {
            var ray = cam.ScreenPointToRay(screenPosition);
            var zeroPlane = new Plane(Vector3.up, Vector3.zero);
            if (!zeroPlane.Raycast(ray, out var enter))
                throw new Exception("camera position raycast Exception");
            return ray.GetPoint(enter);
        }

        [Button]
        private void RefreshInEditor()
        {
            SetRaw(Vector3.zero, minDistance);
        }

        private void Update()
        {
            FollowTarget();
        }

        private void FollowTarget()
        {
            var targetPos = Vector3.zero;
            var finalDist = minDistance;

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

                var camXSizePerDist = (pivotBottomRight.x * 2) / pivotDist;
                var camZSizePerDist = (-pivotBottomRight.z * 2) / pivotDist;

                targetPos = center;
                finalDist = Mathf.Max(xSize / camXSizePerDist, zSize / camZSizePerDist, minDistance);
            }

            var look = Quaternion.AngleAxis(angle, Vector3.right) * Vector3.forward;
            var rot = Quaternion.LookRotation(look, Vector3.up);
            var finalPos = targetPos - look * finalDist;

            var prevPos = transform.position;

            var curPos = Vector3.Lerp(prevPos, finalPos, 0.1f);
            transform.SetPositionAndRotation(curPos, rot);
        }
    }
}