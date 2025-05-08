using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rogue.Ingame.Camera
{
    [ExecuteInEditMode]
    public class ToolCameraController : MonoBehaviour
    {
        private readonly float PhiFixedValue = 48f;
        private readonly float distFixedValue = 28f;

        [SerializeField, PropertyOrder(0)] private Vector2 rotateSpeed = new Vector2(.5f, .3f);
        [SerializeField, PropertyOrder(0)] private float distanceSpeed = 0.5f;

        [Title("Phi")]
        [OnValueChanged(nameof(ValueChanged))]
        [SerializeField, PropertyOrder(1)] private bool phiIsFixed = false;
        [ShowInInspector, PropertyOrder(1)] private float PhiMin => phiIsFixed ? PhiFixedValue : 1f;
        [ShowInInspector, PropertyOrder(1)] private float PhiMax => phiIsFixed ? PhiFixedValue : 89f;

        [Title("Distance")]
        [OnValueChanged(nameof(ValueChanged))]
        [SerializeField, PropertyOrder(2)] private bool distIsFixed = false;
        [ShowInInspector, PropertyOrder(2)] private float DistMin => distIsFixed ? distFixedValue : 2f;
        [ShowInInspector, PropertyOrder(2)] private float DistMax => distIsFixed ? distFixedValue : 60f;

        [Title("Result")]
        [ShowInInspector, PropertyOrder(3)] [ReadOnly] private Vector3 Position { get; set; } = Vector3.zero;
        [ShowInInspector, PropertyOrder(3)] [ReadOnly] private float Theta { get; set; } = -90f;
        [ShowInInspector, PropertyOrder(3)] [ReadOnly] private float Phi { get; set; } = 30f;
        [ShowInInspector, PropertyOrder(3)] [ReadOnly] private float Distance { get; set; } = 40f;


        private UnityEngine.Event Current => UnityEngine.Event.current;
        private UnityEngine.Camera Camera => UnityEngine.Camera.main;

#if UNITY_EDITOR
        void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 120, 40), "Reset Position"))
            {
                Position = Vector3.zero;
            }
            Distance = GUI.HorizontalSlider(new Rect(0, 40, 120, 40), Distance, DistMin, DistMax);

            if (Current.type == EventType.Repaint || Current.type == EventType.Layout)
            {
                EditorUtility.SetDirty(this);
            }

            if (Current.type == EventType.MouseDrag && Current.button == 1)
            {
                RightMouseDrag();
            }

            if (Current.type == EventType.MouseDrag && Current.button == 0)
            {
                LeftMouseDrag();
            }

            CalculateTransform();
        }
#endif

        void RightMouseDrag()
        {
            var delta = Current.delta;
            Theta = Theta - delta.x * rotateSpeed.x;
            Phi = Mathf.Clamp(Phi + delta.y * rotateSpeed.y, PhiMin, PhiMax);
        }

        void LeftMouseDrag()
        {
            var prevPosition = Current.mousePosition - Current.delta;
            var curPosition = Current.mousePosition;
            var prevViewPort = ConvertViewPort(Camera.ScreenToViewportPoint(prevPosition));
            var curViewPort = ConvertViewPort(Camera.ScreenToViewportPoint(curPosition));
            var prevRay = Camera.ViewportPointToRay(prevViewPort);
            var curRay = Camera.ViewportPointToRay(curViewPort);

            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(prevRay, out var enterPrev) && plane.Raycast(curRay, out var enterCur))
            {
                var prevHit = prevRay.GetPoint(enterPrev);
                var curHit = curRay.GetPoint(enterCur);
                Position += prevHit - curHit;
            }
        }

        private Vector3 ConvertViewPort(Vector3 input)
        {
            return new Vector3(input.x, 1f - input.y, 0f);
        }

        void ValueChanged()
        {
            Distance = Mathf.Clamp(Distance, DistMin, DistMax);
            Phi = Mathf.Clamp(Phi, PhiMin, PhiMax);
            CalculateTransform();
        }

        void CalculateTransform()
        {
            var direction = new Vector3(Mathf.Cos(Theta * Mathf.Deg2Rad), 0f, Mathf.Sin(Theta * Mathf.Deg2Rad))
            * Mathf.Cos(Phi * Mathf.Deg2Rad) + new Vector3(0f, Mathf.Sin(Phi * Mathf.Deg2Rad), 0f);

            transform.position = Position + direction * Distance;
            transform.rotation = Quaternion.LookRotation(-direction, Vector3.up);
        }
    }
}
