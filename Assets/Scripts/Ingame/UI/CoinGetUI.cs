using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Rogue.Ingame.UI
{
    public class CoinGetUI : MonoBehaviour
    {
        [SerializeField] [Required] private Animation anim;
        [SerializeField] [Required] private TextMeshProUGUI text;
        [SerializeField] [Required] private RectTransform rect;

        private Action despawnAction;
        private Transform followTm;
        private Vector3 followOffset;
        private UnityEngine.Camera mainCamera;

        public void Show(int amount, Transform tm, Vector3 offset)
        {
            text.text = amount.ToString();
            followTm = tm;
            followOffset = offset;
            anim.Play();
        }

        public void Update()
        {
            if (!anim.isPlaying)
            {
                followTm = null;
                despawnAction();
            }
        }

        private void LateUpdate()
        {
            if (followTm != null)
            {
                if (mainCamera == null)
                    mainCamera = UnityEngine.Camera.main;
                var world = followTm.position + followOffset;
                var viewPort = mainCamera.WorldToViewportPoint(world);
                SetPosition(viewPort);
            }
        }

        public void RegisterDespawn(Action action)
        {
            despawnAction = action;
        }

        private void SetPosition(Vector2 viewPort)
        {
            rect.anchorMin = viewPort;
            rect.anchorMax = viewPort;
            rect.anchoredPosition = Vector2.zero;
        }

    }
}