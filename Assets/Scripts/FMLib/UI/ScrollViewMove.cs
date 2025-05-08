using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace FMLib.UI
{
    public enum ScrollType
    {
        Horizontal, Vertical
    }

    public readonly struct Range
    {
        public readonly float Min;
        public readonly float Max;

        public float Middle => (Min + Max) / 2f;

        public Range(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public static Range operator +(Range r, float val)
        {
            return new Range(r.Min + val, r.Max + val);
        }

        public static Range operator -(Range r, float val)
        {
            return r + (-val);
        }

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, Min, Max);
        }

        public float Size()
        {
            return Max - Min;
        }
    }

    [Required]
    public class ScrollViewMove : MonoBehaviour
    {
        [SerializeField] [Required] private ScrollRect scrollRect;
        [SerializeField] private ScrollType scrollType = ScrollType.Horizontal;
        [SerializeField] float duration = 0.3f;

        private readonly Vector3[] corners = new Vector3[4];

        public void MoveToDirectly(RectTransform rectTransform)
        {
            MoveTo(rectTransform, false);
        }

        public void MoveTween(RectTransform rectTransform)
        {
            MoveTo(rectTransform, true);
        }

        private void MoveTo(RectTransform rectTransform, bool tween)
        {
            var viewPortWorldRange = GetWorldRange(scrollRect.viewport);
            var contentWorldRange = GetWorldRange(scrollRect.content);
            var targetWorldRange = GetWorldRange(rectTransform);

            // min => viewPort기준 right쪽 content와빈공간, max -> viewPort기준 left쪽 content와 빈공간
            Range movableRange = new Range(
                viewPortWorldRange.Max - contentWorldRange.Max,
                viewPortWorldRange.Min - contentWorldRange.Min
            );

            float distance;
            if (targetWorldRange.Size() > viewPortWorldRange.Size())
                distance = viewPortWorldRange.Min - targetWorldRange.Min; // 아무튼 viewPort안에 걸치기 위한 move거리 --> left기준
            else
                distance = viewPortWorldRange.Middle - targetWorldRange.Middle; // 가운데로 옮기기 위한 move 거리

            var totalDistance = movableRange.Clamp(distance);
            Move(totalDistance, tween);
        }

        private Range GetWorldRange(RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners(corners);
            var min = GetCornersMinValue(corners);
            var max = GetCornersMaxValue(corners);

            return new Range(min, max);
        }

        // 배열순서: 왼쪽 아래부터 시계방향
        // 0 - 왼쪽아래, 1 - 왼쪽위, 2 - 오른쪽위, 3 - 오른쪽아래
        private float GetCornersMinValue(Vector3[] corners)
        {
            if (scrollType == ScrollType.Horizontal)
                return corners[0].x;

            return corners[0].y;
        }

        private float GetCornersMaxValue(Vector3[] corners)
        {
            if (scrollType == ScrollType.Horizontal)
                return corners[2].x;

            return corners[2].y;
        }

        private void Move(float distance, bool tween)
        {
            scrollRect.StopMovement();

            var contentTransform = scrollRect.content.transform;
            if (tween)
                StartCoroutine(MoveTween(contentTransform, distance, duration));
            else
                MoveByDirectly(contentTransform, distance);
        }

        IEnumerator MoveTween(Transform tm, float distance, float time)
        {
            var startPos = tm.position;
            var targetPos = startPos + (scrollType == ScrollType.Horizontal
                ? new Vector3(distance, 0f, 0f)
                : new Vector3(0f, distance, 0f));

            var elapsed = Time.deltaTime;
            while (elapsed < time)
            {
                var t = elapsed / time;
                tm.position = Vector3.Lerp(startPos, targetPos, t);

                yield return null;
                elapsed += Time.deltaTime;
            }
            tm.position = targetPos;
        }

        private void MoveByDirectly(Transform tm, float distance)
        {
            var pos = tm.position;

            if (scrollType == ScrollType.Horizontal)
                pos.x += distance;
            else
                pos.y += distance;

            tm.position = pos;
        }
    }
}