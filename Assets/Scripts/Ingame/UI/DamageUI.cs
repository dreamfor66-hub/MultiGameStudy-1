using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Rogue.Ingame.UI
{
    public class DamageUI : MonoBehaviour
    {
        [SerializeField] [Required] private Animation anim;
        [SerializeField] [Required] private TextMeshProUGUI text;
        [SerializeField] [Required] private RectTransform rect;

        [SerializeField] private Color damageColor;
        [SerializeField] private Color criticalColor;
        [SerializeField] private Color healColor;


        private Action despawnAction;
        private Vector3 position;
        private UnityEngine.Camera mainCamera;

        public void ShowDamage(int damage, bool critical, Vector3 position)
        {
            text.text = damage.ToString();
            text.fontSize = critical ? 60 : 40;
            text.color = critical ? criticalColor : damageColor;
            this.position = position;
            anim.Play();
        }

        public void ShowHeal(int amount, Vector3 position)
        {
            text.text = amount.ToString();
            text.fontSize = 40;
            text.color = healColor;
            this.position = position;
            anim.Play();
        }

        public void Update()
        {
            if (!anim.isPlaying)
                despawnAction();
            else
                UpdatePosition();
        }

        public void RegisterDespawn(Action action)
        {
            despawnAction = action;
        }

        public void SetPosition(Vector2 viewport)
        {
            rect.anchorMin = viewport;
            rect.anchorMax = viewport;
            rect.anchoredPosition = Vector2.zero;
        }

        private void UpdatePosition()
        {
            if (mainCamera == null)
                mainCamera = UnityEngine.Camera.main;
            var viewPort = mainCamera.WorldToViewportPoint(position);
            SetPosition(viewPort);
        }
    }
}