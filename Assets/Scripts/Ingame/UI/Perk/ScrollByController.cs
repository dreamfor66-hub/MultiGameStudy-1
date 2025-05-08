using Rogue.Ingame.Input;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Rogue.Ingame.UI.Perk
{
    public class ScrollByController : MonoBehaviour
    {
        [Required] [SerializeField] private ScrollRect scrollRect;
        [Required] [SerializeField] private RectTransform viewPortRect;
        [Required] [SerializeField] private RectTransform contentRect;
        [SerializeField] private float moveSpeed = 800f;

        private void Update()
        {
            var viewportLength = viewPortRect.rect.height;
            var contentLength = contentRect.rect.height;
            var movableLength = contentLength - viewportLength;
            var speed = movableLength > 0 ? moveSpeed / movableLength : 0f;
            var vertical = InputDetector.GetVertical();
            scrollRect.verticalNormalizedPosition =
                Mathf.Clamp01(scrollRect.verticalNormalizedPosition + vertical * speed * Time.deltaTime);
        }
    }
}