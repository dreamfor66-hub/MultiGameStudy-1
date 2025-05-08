using Rogue.Ingame.Character;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI.Status
{
    public class MonsterUIAnchor : MonoBehaviour
    {
        [SerializeField] [Required] private CharacterBehaviour character;

        private MonsterUI ui;
        private UnityEngine.Camera mainCamera;

        private void Start()
        {
            if (MonsterUIPool.Instance != null)
            {
                ui = MonsterUIPool.Instance.Get();
                ui.Init(character);
            }
            mainCamera = UnityEngine.Camera.main;
        }

        private void LateUpdate()
        {
            var viewPortPosition = mainCamera.WorldToViewportPoint(this.transform.position);
            ui.SetPosition(viewPortPosition);
        }

        private void OnDestroy()
        {
            if (ui != null)
            {
                ui.Release();
                MonsterUIPool.Instance.Return(ui);
            }
        }

        public void Reset()
        {
            character = GetComponentInParent<CharacterBehaviour>();
        }
    }
}
