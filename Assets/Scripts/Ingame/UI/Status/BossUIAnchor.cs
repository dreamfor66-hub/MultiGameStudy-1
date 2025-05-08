using Rogue.Ingame.Character;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rogue.Ingame.UI.Status
{
    public class BossUIAnchor : MonoBehaviour
    {
        [SerializeField] [Required] private CharacterBehaviour character;
        [SerializeField] private string name;

        private void Awake()
        {
            if (BossUI.Instance != null)
            {
                BossUI.Instance.Bind(character, name);
            }
        }

        private void OnDestroy()
        {
            if (BossUI.Instance != null)
            {
                BossUI.Instance.Release(character);
            }
        }

        public void Reset()
        {
            character = GetComponentInParent<CharacterBehaviour>();
            name = "이름을 입력하세요.";
        }
    }
}